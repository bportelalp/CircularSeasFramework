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
using CircularSeas.DB.Entities;
using CircularSeas.DB.Context;
using CircularSeas.Models;
using Microsoft.AspNetCore.Hosting;
using System.Globalization;
using CircularSeas.DB;
using CircularSeas.Cloud.Server.Helpers;
using CircularSeas.Cloud.Server.SlicerEngine;

namespace CircularSeas.Cloud.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProcessController : Controller
    {
        // Service access
        private readonly Log _log;
        private readonly AppSettings _appSettings;
        private readonly ISlicerCLI _slicer;
        private readonly Tools _tools;
        private readonly IWebHostEnvironment _env;

        // Database context
        private readonly CircularSeasContext _DBContext;
        private readonly DbService _DbService;

        public ProcessController(Log log, IOptions<AppSettings> appSettings, CircularSeasContext circularSeasContext, Tools tools, IWebHostEnvironment env, ISlicerCLI slicer, DbService dbService)
        {
            // Assignment and initialization of services
            this._log = log;
            this._tools = tools;
            this._env = env;
            this._appSettings = appSettings.Value;
            this._slicer = slicer;
            this._DBContext = circularSeasContext;
            this._DbService = dbService;
        }

        /// <summary>
        /// Function to test the connection status to the API
        /// </summary>
        /// <returns></returns>
        [HttpGet] public async Task<IActionResult> GetConnectionStatus()
        {
            return Ok("You are correctly connected to the API");
        }

        [HttpGet("db")]
        public async Task<IActionResult> TestRoute()
        {
            var result = await _DbService.GetMaterials();
            return Ok(result);
        }

        /// <summary>
        ///  Getting printer information, materials and assistance to the selection of materials.
        /// </summary>
        /// <param name="printerId"> Name of the printer </param>
        /// <returns> An object with printer, materials and topsis data </returns>
        [HttpGet("printerinfo/{PrinterID}")]
        public async Task<IActionResult> GetInfoPrinter([FromRoute] string printerId)
        {
            CircularSeas.Models.DTO.PrintDTO dto = new CircularSeas.Models.DTO.PrintDTO();
            try
            {
                dto.Printer = await _DbService.GetPrinterInfo(printerId);
                dto.Materials = await _DbService.GetMaterials(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
            return Ok(dto);
        }



        /// <summary>
        /// Automatic machine code generation (STL to Gcode converter/slicer)
        /// </summary>
        /// <param name="printer">ID Printer, a string with its name </param>
        /// <param name="material">ID Material, a string with its name </param>
        /// <param name="quality">ID Print Profile/Quality, a string with its name </param>
        /// <param name="support"> Support structure generation enablement </param>
        /// <returns> The Gcode generated file </returns>
        [HttpPost("convert")]
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
                    .Where(p => p.ModelName == printer)
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
        
        /// <summary>
        /// This function upload a STL file.
        /// </summary>
        /// <returns></returns>
        [HttpPost("uploadSTL")]
        public async Task<IActionResult> PostUploadSTL()
        {
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
            var streamSTLFile = new FileStream(fullPathSTL, FileMode.Create);
            file.CopyTo(streamSTLFile);
            streamSTLFile.Close();
            _log.logWrite("File reception completed (" + tictoc.ElapsedMilliseconds + "ms): " + NameSTL + extension);

            return Ok("Upload completed");
        }
    }

}
