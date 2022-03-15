using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace CircularSeas.Tools.BundleConfigurator
{
    class BundleConfigurator
    {

        static void Main(string[] args)
        {
            Task.Run(Llamada);


            Console.Read();

        }


        public static async Task Llamada()
        {
            await Task.Delay(5000);
            var Http = new HttpClient();
            Http.BaseAddress = new Uri("http://localhost:5000");

            using (var content = new MultipartFormDataContent("Upload_" + Guid.NewGuid()))
            {
                //using (var ms = new MemoryStream())
                //{
                //    await slicerForm.STL.OpenReadStream().CopyToAsync(ms);
                //    ms.Seek(0, SeekOrigin.Begin);
                //    var stream = new StreamContent(ms);
                //    stream.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                //    content.Add(stream);
                //    var query = QueryHelpers.AddQueryString("api/process/slice",
                //        new Dictionary<string, string>()
                //        {
                //            {"printer",slicerForm.Printer },
                //            {"filament", _filaments.Find(f => f.MaterialFK == slicerForm.Material).Name },
                //            {"print",_filaments.SelectMany(f => f.CompatiblePrints).Where(f => f.Id == slicerForm.Print).FirstOrDefault().Name },
                //            {"support", slicerForm.Support == "Yes"? true.ToString():false.ToString() }
                //        });
                //    var response = await Http.PostAsync(query, content);
                //}

                var fs = File.OpenRead("C:\\Users\\brportela\\Downloads\\xyzCalibration_cube.stl");
                var ms = new MemoryStream();

                fs.Position = 0;
                fs.CopyTo(ms);

                var streamContent = new StreamContent(ms);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/sla");
                content.Add(streamContent);
                var query = QueryHelpers.AddQueryString("api/process/slice",
                        new Dictionary<string, string>()
                        {
                            {"printer","CS_Ultimaker2plus"},
                            {"filament","CS_PLA" },
                            {"print","CS_Estandar-0.20mm" },
                            {"support", "true" }
                        });
                var response = await Http.PostAsync(query, content);
            }
            /*
             * Esta aplicación de consola simplifica el fichero origen realizado por Prusa Slicer para crear
             * tres archivos .ini independientes que permitan trabajar a CircularSeasWebAPI.
             * Reemplace las ruta siguiente por la ubicación de su archivo bundle
             */
            //string bundlePath = @"C:\Documentos\TFM\Slicer_data\quality_bundle\PrusaSlicer_config_bundle.ini";


            ////Las otras rutas
            //string printPath = Path.GetDirectoryName(bundlePath) + "\\print.ini";
            //string filamentPath = Path.GetDirectoryName(bundlePath) + "\\filament.ini";
            //string printerPath = Path.GetDirectoryName(bundlePath) + "\\printer.ini";


            ////Métrica de tiempo
            //Stopwatch tictoc = new Stopwatch();
            //tictoc.Start();
            //Console.WriteLine("Iniciando Creación de ficheros independientes...");
            ////Tomar el bundle de .ini
            //string[] bundle = System.IO.File.ReadAllLines(bundlePath);
            //Console.WriteLine("Bundle cargado en memoria : " + bundlePath);

            ////Construir ienumerables para la localización de propiedades de cada elemento
            //List<string> iniList = new List<string>();
            //List<string> iniPrint = new List<string>();
            //List<string> iniFilament = new List<string>();
            //List<string> iniPrinter = new List<string>();
            ////Variables auxiliares para localizar los presets
            //bool[] PresetsLocated = new bool[3]; //Confirmación de "paquete incluido"
            //int found = 0; //Localizador de paquete 0-ninguno 1-print 2-filament 3-printer

            ////Bucle para localizar los presets en el bundle
            //foreach (string line in bundle) {
            //    //Si hay un paquete localizado, copia todas las líneas en el dict hasta la línea vacía
            //    if (found > 0) {
            //        if (found == 1) {
            //            iniPrint.Add(line);
            //            if (line == "") {
            //                found = 0;
            //                Console.WriteLine("     Copiado en print!");
            //            }
            //        }
            //        else if (found == 2) {
            //            iniFilament.Add(line);
            //            if (line == "") {
            //                found = 0;
            //                Console.WriteLine("     Copiado en filament!");
            //            }
            //        }
            //        else if (found == 3) {
            //            iniPrinter.Add(line);
            //            if (line == "") {
            //                found = 0;
            //                Console.WriteLine("     Copiado en printer!");
            //            }
            //        }
            //    }
            //    //Localizar etiquetas de presets
            //    if (line.StartsWith("[print:")) {
            //        found = 1;
            //        iniPrint.Add(line);
            //        Console.WriteLine("Localizado preajuste de print: " + line);
            //    }
            //    if (line.StartsWith("[filament:")) {
            //        found = 2;
            //        iniFilament.Add(line);
            //        Console.WriteLine("Localizado preajuste de filament: " + line);
            //    }
            //    if (line.StartsWith("[printer:")) {
            //        found = 3;
            //        iniPrinter.Add(line);
            //        Console.WriteLine("Localizado preajuste de printer: " + line);
            //    }
            //}

            //Console.WriteLine("Búsqueda terminada, copiando en:");
            ////Escribir cada linea en un fichero
            //System.IO.File.WriteAllLines(printPath, iniPrint);
            //Console.WriteLine("   Print: " + printPath);
            //System.IO.File.WriteAllLines(filamentPath, iniFilament);
            //Console.WriteLine("   Filament: " + filamentPath);
            //System.IO.File.WriteAllLines(printerPath, iniPrinter);
            //Console.WriteLine("   Printer: " + printerPath);
            //tictoc.Stop();
            //System.Console.WriteLine("Completado en: " + tictoc.ElapsedMilliseconds + " ms");
        }

    }
}
