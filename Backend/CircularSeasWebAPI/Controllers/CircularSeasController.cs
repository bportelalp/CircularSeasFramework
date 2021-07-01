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
using CircularSeasWebAPI.Models;
using CircularSeasWebAPI.Helpers;
using CircularSeasWebAPI.Classes;

namespace CircularSeasWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CircularSeasController : Controller
    {
        // Service access
        private readonly Log log;
        private readonly AppSettings appSettings;
        private readonly Utilities utilities;
        // Database context
        private readonly CircularSeasContext DataContext;

        public CircularSeasController(Log log, IOptions<AppSettings> appSettings, CircularSeasContext circularSeasContext, Utilities utilities)
        {
            // Assignment and initialization of services
            this.log = log;
            this.utilities = utilities;
            this.appSettings = appSettings.Value;

            this.DataContext = circularSeasContext;
        }

        /// <summary>
        ///  Getting printer information, materials and assistance to the selection of materials.
        /// </summary>
        /// <param name="PrinterID"> Name of the printer </param>
        /// <returns> An object with printer, materials and topsis data </returns>
        [HttpGet("Printer/{PrinterID}")]
        public async Task<JsonResult> GetInfoPrinter([FromRoute] string PrinterID)
        {

            Data dataSet = new Data();

            // Search in the database of the values of the Properties, characteristics and impact that each material has on the selection of materials.
            InfoTopsis topsisData = new InfoTopsis();
            topsisData.FeaturesLabels = await DataContext.Features
                .OrderBy(s => s.Id)
                .Select(s => s.Name)
                .ToArrayAsync();
            topsisData.PropertiesLabels = await DataContext.Properties
                .OrderBy(s => s.Id)
                .Select(s => s.Name)
                .ToArrayAsync();
            topsisData.PositiveImpact = await DataContext.Properties
                .OrderBy(s => s.Id)
                .Select(s => s.PositiveImpact)
                .ToArrayAsync();

            dataSet.InfoTopsis = topsisData;

            // Search in the DB for the list of all materials 
            var materialsBBDDlist = await DataContext.Materials.ToListAsync();

            // Conversion of the DB Materials model into the reduced "Filaments" class 
            List<Filament> filamentsList = new List<Filament>();
            foreach (var item in materialsBBDDlist)
            {
                filamentsList.Add(new Filament
                {
                    Name = item.Name,
                    Description = item.Description,
                    Features = await DataContext.FeatureMats
                        .Where(s => s.IdMaterial == item.Id)
                        .OrderBy(s => s.IdFeature)
                        .Select(s => s.Value)
                        .ToArrayAsync(),
                    Properties = await DataContext.PropMats
                        .Where(s => s.IdMaterial == item.Id)
                        .OrderBy(s => s.IdProperty)
                        .Select(s => s.Value)
                        .ToArrayAsync(),
                    Stock = 0
                });
            }
            dataSet.Filaments = filamentsList.ToArray();

            // Getting printer compatibility with available printer/quality profiles and default filament diameter. This will appear as user selectable in the application.
            var comp = (JObject)JsonConvert.DeserializeObject(System.IO.File.ReadAllText(appSettings.dataPath + "\\compatibilidades.json"));

            // Fill the "Printer" object with the compatibility and filament diameter data
            dataSet.Printer = new Printer
            {
                Nombre = PrinterID,
                Profiles = comp[PrinterID]["compatible_quality"].ToObject<string[]>(),
                Filament_diameter = comp[PrinterID]["filament_diameter"].ToObject<double>()
            };
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
                log.logWrite("New slicing request");
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
                log.logWrite("\n");
                log.logWrite("CAM process start");

                // STL file identification and storage in memory
                var NombreSTL = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                NombreSTL = NombreSTL.Split(".")[0];
                string extension = Path.GetExtension(file.FileName);
                var fullPathSTL = appSettings.stlFolderPath + "\\" + NombreSTL + extension;
                var streamSTLFile = new FileStream(fullPathSTL, FileMode.Create);
                file.CopyTo(streamSTLFile);
                streamSTLFile.Close();
                log.logWrite("File reception completed (" + tictoc.ElapsedMilliseconds + "ms): " + NombreSTL + extension);

                //Crear el archivo de configuración
                log.logWrite("Creating configuration file");


                Dictionary<string, string> paramsDict = new Dictionary<string, string>();

                // Filament diameter overwrite
                var comp = (JObject)JsonConvert.DeserializeObject(System.IO.File.ReadAllText(appSettings.dataPath + "\\compatibilidades.json"));

                paramsDict.Add("support_material", bool.Parse(support) ? "1" : "0");
                paramsDict.Add("filament_diameter", comp[printer]["filament_diameter"].ToObject<string>());

                var iniName = utilities.ConfigFileCreator(printer, material, quality, paramsDict);

                var iniPath = appSettings.inisPath + "\\" + iniName;
                log.logWrite("File created (" + tictoc.ElapsedMilliseconds + "ms) on: " + iniPath);

                // G code generation process
                log.logWrite("Slicing with PrusaSlicer");
                string attributes = "--slice " + fullPathSTL + " --load \"" + iniPath + "\" -o " + appSettings.gCodeFolderPath + "\\" + NombreSTL + ".gcode";
                log.logWrite("Command: " + attributes);

                // Execution by CMD 
                var resultConsola = utilities.ExecuteCommand(attributes);
                if (resultConsola != null)
                {
                    log.logWrite("The request could not be completed. Return status code: " + HttpStatusCode.PreconditionFailed);
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, resultConsola);
                }
                log.logWrite("Slicing process end");

                // Get G-code file from storage folder 
                var docDestination = appSettings.gCodeFolderPath + "\\" + NombreSTL + ".gcode";
                var streamGcodeFile = new FileStream(docDestination, FileMode.Open, FileAccess.Read);
                // Writing results in the log file
                log.logWrite("File copied on " + docDestination);
                tictoc.Stop();
                log.logWrite("Elapsed time to complete G-code generation: " + tictoc.ElapsedMilliseconds + " ms");
                //Devolver o ficheiro convertido
                return Ok(streamGcodeFile);

            }
            catch (Exception ex)
            {
                log.logWrite(ex.ToString());
                return BadRequest(ex.ToString());
                //return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }
    }

}
