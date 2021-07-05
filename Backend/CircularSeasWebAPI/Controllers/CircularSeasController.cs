using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Net;
using Microsoft.EntityFrameworkCore;
using CircularSeasWebAPI.Entities;
using CircularSeasWebAPI.Helpers;
using CircularSeas.Models;
using Microsoft.AspNetCore.Hosting;
using CircularSeasWebAPI.SlicerEngine;
using System.Globalization;

namespace CircularSeasWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CircularSeasController : Controller
    {
        // Service access
        private readonly Log _log;
        private readonly AppSettings _appsSettings;
        private readonly ISlicerCLI _slicer;
        private readonly Tools _tools;
        private readonly IWebHostEnvironment _env;

        // Database context
        private readonly CircularSeasContext _DBContext;

        public CircularSeasController(Log log, IOptions<AppSettings> appSettings, CircularSeasContext circularSeasContext, Tools tools, IWebHostEnvironment env, ISlicerCLI slicer)
        {
            // Assignment and initialization of services
            this._log = log;
            this._tools = tools;
            this._env = env;
            this._appsSettings = appSettings.Value;
            this._slicer = slicer;
            this._DBContext = circularSeasContext;
        }

        /// <summary>
        ///  Getting printer information, materials and assistance to the selection of materials.
        /// </summary>
        /// <param name="PrinterID"> Name of the printer </param>
        /// <returns> An object with printer, materials and topsis data </returns>
        [HttpGet("Printer/{PrinterID}")]
        public async Task<JsonResult> GetInfoPrinter([FromRoute] string PrinterID)
        {
            CircularSeas.Models.DTO.DataDTO dataSet = new CircularSeas.Models.DTO.DataDTO();

            // Search in the database of the values of the Properties, characteristics and impact that each material has on the selection of materials.
            CircularSeas.Models.InfoTopsis infoTopsis = new CircularSeas.Models.InfoTopsis();
            try
            {
                infoTopsis.FeaturesNames = await _DBContext.Features
                .OrderBy(s => s.Id)
                .Select(s => s.Name)
                .ToArrayAsync();
                infoTopsis.PropertiesNames = await _DBContext.Properties
                    .OrderBy(s => s.Id)
                    .Select(s => s.Name)
                    .ToArrayAsync();
                infoTopsis.ImpactPositive = await _DBContext.Properties
                    .OrderBy(s => s.Id)
                    .Select(s => s.PositiveImpact)
                    .ToArrayAsync();
                dataSet.InfoTopsis = infoTopsis;
                //Get all profiles availables.
                var printers = await _DBContext.Printers
                    .Where(pr => pr.Name == PrinterID)
                    .Include(pr => pr.PrinterProfiles)
                    .FirstOrDefaultAsync();
                dataSet.Printer = Mapper.Repo2Domain(printers);

                // Search in the DB for the list of all materials 
                var materialsBBDDlist = await _DBContext.Materials
                    .Include(mat => mat.FeatureMats.OrderBy(f => f.IdFeature))
                    .Include(mat => mat.PropMats.OrderBy(p => p.IdProperty))
                    .ToListAsync();

                // Conversion of the DB Materials model into the reduced "Filaments" class 
                dataSet.Filaments = Mapper.Repo2Domain(materialsBBDDlist).ToArray();

            }
            catch (Exception)
            {

                throw;
            }
            
            return Json(dataSet);
        }

        /// <summary>
        /// Automatic machine code generation (STL to Gcode converter/slicer)
        /// </summary>
        /// <param name="printer">ID Printer, a string with its name </param>
        /// <param name="material">ID Material, a string with its name </param>
        /// <param name="quality">ID Print Profile/Quality, a string with its name </param>
        /// <param name="support"> Support structure generation enablement </param>
        /// <returns> The Gcode generated file </returns>
        [HttpPost("upload")]
        public async Task<IActionResult> PostUpload([FromQuery(Name = "printer")] string printer, [FromQuery(Name = "material")] string material,
                [FromQuery(Name = "quality")] string quality, [FromQuery(Name = "support")] string support)
        {

            try
            {
                _log.logWrite("New slicing request");
                // Checking that the parameters are correct 
                if (String.IsNullOrEmpty(printer) || String.IsNullOrEmpty(material) || String.IsNullOrEmpty(quality) || String.IsNullOrEmpty(support))
                {
                    // Throw parameter error exception 
                    throw new NullReferenceException("Some of the parameters are null");
                }

                // Getting the STL file
                Microsoft.AspNetCore.Http.IFormFile file;
                if (Request.Form.Files.Count == 0)
                {
                    return NotFound("STL file missing");
                }
                else
                {
                    file = Request.Form.Files[0];
                }

                // Start time calculation metrics
                Stopwatch tictoc = new Stopwatch();
                tictoc.Start();
                _log.logWrite("\n");
                _log.logWrite("CAM process start");

                // STL file identification and storage in memory
                var NameSTL = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                NameSTL = NameSTL.Split(".")[0];
                string extension = Path.GetExtension(file.FileName);
                var fullPathSTL = _tools.GetWebPath(WebFolder.STL) + NameSTL + extension;
                var fullPathGCODE = _tools.GetWebPath(WebFolder.GCode) + NameSTL + ".gcode";
                var streamSTLFile = new FileStream(fullPathSTL, FileMode.Create);
                file.CopyTo(streamSTLFile);
                streamSTLFile.Close();
                _log.logWrite("File reception completed (" + tictoc.ElapsedMilliseconds + "ms): " + NameSTL + extension);

                //Crear el archivo de configuración
                _log.logWrite("Creating configuration file");


                Dictionary<string, string> paramsDict = new Dictionary<string, string>();

                // Filament diameter overwrite
                double filamentDiameter = await _DBContext.Printers
                    .Where(p => p.Name == printer)
                    .Select(p => p.FilamentDiameter)
                    .FirstOrDefaultAsync();

                paramsDict.Add("support_material", bool.Parse(support) ? "1" : "0");
                paramsDict.Add("filament_diameter", filamentDiameter.ToString(CultureInfo.InvariantCulture));

                var iniName = _tools.ConfigFileCreator(printer, material, quality, paramsDict);

                var iniPath = _tools.GetWebPath(WebFolder.INI) + iniName;
                _log.logWrite("File created (" + tictoc.ElapsedMilliseconds + "ms) on: " + iniPath);

                // G code generation process
                _log.logWrite("Slicing with PrusaSlicer");
                string attributes = "--slice " + fullPathSTL + " --load \"" + iniPath + "\" -o " + fullPathGCODE;
                _log.logWrite("Command: " + attributes);

                // Execution by CMD 
                string resultConsola = _slicer.ExecuteCommand(attributes);
                if (resultConsola != null)
                {
                    _log.logWrite("The request could not be completed. Return status code: " + HttpStatusCode.PreconditionFailed);
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, resultConsola);
                }
                _log.logWrite("Slicing process end");

                // Get G-code file from storage folder 
                var streamGcodeFile = new FileStream(fullPathGCODE, FileMode.Open, FileAccess.Read);
                // Writing results in the log file
                _log.logWrite("File copied on " + fullPathGCODE);
                tictoc.Stop();
                _log.logWrite("Elapsed time to complete G-code generation: " + tictoc.ElapsedMilliseconds + " ms");
                //Devolver o ficheiro convertido
                return Ok(streamGcodeFile);

            }
            catch (Exception ex)
            {
                _log.logWrite(ex.ToString());
                return BadRequest(ex.ToString());
                //return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }
    }

}
