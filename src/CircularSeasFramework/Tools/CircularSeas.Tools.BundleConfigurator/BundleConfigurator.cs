using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CircularSeas.Tools.BundleConfigurator {
    class BundleConfigurator {

        static void Main(string[] args) {
            /*
             * Esta aplicación de consola simplifica el fichero origen realizado por Prusa Slicer para crear
             * tres archivos .ini independientes que permitan trabajar a CircularSeasWebAPI.
             * Reemplace las ruta siguiente por la ubicación de su archivo bundle
             */
            string bundlePath = @"C:\Documentos\TFM\Slicer_data\quality_bundle\PrusaSlicer_config_bundle.ini";


            //Las otras rutas
            string printPath = Path.GetDirectoryName(bundlePath) + "\\print.ini";
            string filamentPath = Path.GetDirectoryName(bundlePath) + "\\filament.ini";
            string printerPath = Path.GetDirectoryName(bundlePath) + "\\printer.ini";


            //Métrica de tiempo
            Stopwatch tictoc = new Stopwatch();
            tictoc.Start();
            Console.WriteLine("Iniciando Creación de ficheros independientes...");
            //Tomar el bundle de .ini
            string[] bundle = System.IO.File.ReadAllLines(bundlePath);
            Console.WriteLine("Bundle cargado en memoria : " + bundlePath);

            //Construir ienumerables para la localización de propiedades de cada elemento
            List<string> iniList = new List<string>();
            List<string> iniPrint = new List<string>();
            List<string> iniFilament = new List<string>();
            List<string> iniPrinter = new List<string>();
            //Variables auxiliares para localizar los presets
            bool[] PresetsLocated = new bool[3]; //Confirmación de "paquete incluido"
            int found = 0; //Localizador de paquete 0-ninguno 1-print 2-filament 3-printer

            //Bucle para localizar los presets en el bundle
            foreach (string line in bundle) {
                //Si hay un paquete localizado, copia todas las líneas en el dict hasta la línea vacía
                if (found > 0) {
                    if (found == 1) {
                        iniPrint.Add(line);
                        if (line == "") {
                            found = 0;
                            Console.WriteLine("     Copiado en print!");
                        }
                    }
                    else if (found == 2) {
                        iniFilament.Add(line);
                        if (line == "") {
                            found = 0;
                            Console.WriteLine("     Copiado en filament!");
                        }
                    }
                    else if (found == 3) {
                        iniPrinter.Add(line);
                        if (line == "") {
                            found = 0;
                            Console.WriteLine("     Copiado en printer!");
                        }
                    }
                }
                //Localizar etiquetas de presets
                if (line.StartsWith("[print:")) {
                    found = 1;
                    iniPrint.Add(line);
                    Console.WriteLine("Localizado preajuste de print: " + line);
                }
                if (line.StartsWith("[filament:")) {
                    found = 2;
                    iniFilament.Add(line);
                    Console.WriteLine("Localizado preajuste de filament: " + line);
                }
                if (line.StartsWith("[printer:")) {
                    found = 3;
                    iniPrinter.Add(line);
                    Console.WriteLine("Localizado preajuste de printer: " + line);
                }
            }

            Console.WriteLine("Búsqueda terminada, copiando en:");
            //Escribir cada linea en un fichero
            System.IO.File.WriteAllLines(printPath, iniPrint);
            Console.WriteLine("   Print: " + printPath);
            System.IO.File.WriteAllLines(filamentPath, iniFilament);
            Console.WriteLine("   Filament: " + filamentPath);
            System.IO.File.WriteAllLines(printerPath, iniPrinter);
            Console.WriteLine("   Printer: " + printerPath);
            tictoc.Stop();
            System.Console.WriteLine("Completado en: " + tictoc.ElapsedMilliseconds + " ms");
        }

    }
}
