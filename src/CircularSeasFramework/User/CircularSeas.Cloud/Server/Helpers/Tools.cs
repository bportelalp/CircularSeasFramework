using CircularSeas.Adapters;
using CircularSeas.Infrastructure.Logger;
using CircularSeas.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircularSeas.Cloud.Server.Helpers
{
    public class Tools
    {

        // Service access
        private readonly ILog _log;
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// Enumeration containing three options to indicate what is searching
        /// </summary>
        enum searchingOption
        {
            Printer,
            Filament,
            Print_Profile,
        }

        /// <summary>
        /// Constructor of the Utilities Class
        /// </summary>
        /// <param name="log"> Injection of the Log service </param>
        /// <param name="appSettings"> Injection of the appSettings service </param>
        public Tools(ILog log, IWebHostEnvironment env)
        {
            this._log = log;
            this._env = env;
        }
        #region "Methods"
        /// <summary>
        /// Creation of the PrusaSlicer configuration file to print
        /// </summary>
        /// <param name="_printer"> Name of the printer selected </param>
        /// <param name="_filament"> Name of the material selected </param>
        /// <param name="_profile"> Name of the print profile (quality) selected </param>
        /// <param name="_params"> Dictionary that contains all the specific parameters to overwrite the ones in the .ini files </param>
        /// <returns> The name of the file generated (inside this function) that contains the configuration selected</returns>
        public string ConfigFileCreator(string _printer, string _filament, string _profile, Dictionary<string, string> _params)
        {

            // Collections contruction for the propierties (key-vaue pair) location
            List<string> iniList = new List<string>();
            SortedDictionary<string, string> iniDict = new SortedDictionary<string, string>(); //Alphabetic order   
                                                                                               // int bundleFileFound = 0; // Indication of the bundle found (.ini files): 0-completed, 1-printer, 2-filament, 3-print profiles

            // Name generation of the file configuration (.ini file).
            string iniName = _printer + "_" + _filament + "_" + _profile + ".ini";

            // Loading printer settings
            loadConfigParams(_printer, "printer.ini", searchingOption.Printer, ref iniDict);
            // Loading filament settings
            loadConfigParams(_filament, "filament.ini", searchingOption.Filament, ref iniDict);
            // Loading print profiles settings
            loadConfigParams(_profile, "print.ini", searchingOption.Print_Profile, ref iniDict);


            // Loop to induce/overwrite pairs of values to be specifically changed
            foreach (KeyValuePair<string, string> parameter in _params)
            {
                if (iniDict.ContainsKey(parameter.Key.Trim()))
                {
                    iniDict[parameter.Key] = parameter.Value;
                }
                else
                {
                    iniDict.Add(parameter.Key, parameter.Value);
                }
            }

            // Load all keys into a list, value separated by = 
            foreach (KeyValuePair<string, string> propiedad in iniDict)
            {
                iniList.Add(propiedad.Key + " = " + propiedad.Value);
            }

            // Write each line to a file
            System.IO.File.WriteAllLines(this.GetWebPath(WebFolder.INI) + iniName, iniList);
            return iniName;
        }

        public async Task<string> ConfigFileCreator(IDbService DbContext, string printer, string filament, string print, Dictionary<string, string> extraParams)
        {

            // Collections contruction for the propierties (key-vaue pair) location
            List<string> iniList = new List<string>();
            SortedDictionary<string, string> iniDict = new SortedDictionary<string, string>(); //Alphabetic order   
                                                                                               // int bundleFileFound = 0; // Indication of the bundle found (.ini files): 0-completed, 1-printer, 2-filament, 3-print profiles

            // Name generation of the file configuration (.ini file).
            string iniName = DateTime.Now.ToString("yyMMdd") + "_" + Guid.NewGuid() + ".ini";

            iniDict = await DbContext.GetSlicerConfig(printer, filament, print);

            // Loop to induce/overwrite pairs of values to be specifically changed
            foreach (KeyValuePair<string, string> parameter in extraParams)
            {
                if (iniDict.ContainsKey(parameter.Key.Trim()))
                {
                    iniDict[parameter.Key] = parameter.Value;
                }
                else
                {
                    iniDict.Add(parameter.Key, parameter.Value);
                }
            }

            iniList.Add($"# Generated by CircularSeas Cloud Service on {DateTime.Now.ToString()} " +
                $"for printer: {printer}, " +
                $"filament {filament} " +
                $"and print: {print}");
            iniList.Add("");

            // Load all keys into a list, value separated by = 
            foreach (KeyValuePair<string, string> propiedad in iniDict)
            {
                iniList.Add(propiedad.Key + " = " + propiedad.Value);
            }

            // Write each line to a file
            System.IO.File.WriteAllLines(this.GetWebPath(WebFolder.INI) + iniName, iniList);
            return iniName;
        }



        public string GetWebPath(WebFolder folder)
        {
            return _env.WebRootPath + "\\" + folder.ToString() + "\\";
        }
        #endregion

        #region "Functions"
        /// <summary>
        /// Search in the configuration files for a selected printer, a selected material (filament) or a selected quality (profile) and load all the parameters in a dictionary passed by reference. 
        /// </summary>
        /// <param name="selection"> It stands for the printer, filament or quality (print profile) selected. </param>
        /// <param name="fileName"> Contains the configuration file of all printers/filaments/qualities </param>
        /// <param name="option"> Indicates whether the printer, media, or print profile is being searched. </param>
        /// <param name="iniDict"> [REF] Dictionary that will contai all the parameters of the .ini files </param>
        private void loadConfigParams(string selection, string fileName, searchingOption option, ref SortedDictionary<string, string> iniDict)
        {
            // Loading the printer bundle file (.ini)
            string[] bundle = System.IO.File.ReadAllLines(this.GetWebPath(WebFolder.Bundle) + fileName);

            bool found = false;
            // Searching the specified _printer propierties
            foreach (string line in bundle)
            {
                // If there is a bundle located, copy all the lines in the dictionary up to the empty line 
                if (found == true)
                {
                    if (line == "")
                    {
                        // If the search is found and an empty line appears, it means that it has just loaded all the parameters of that .ini file for the required selection.
                        break;
                    }
                    else
                    {
                        // Divide into key pair, value 
                        string[] keyvalue = line.Split("=");
                        // Duplicate elements checking in dictionary
                        if (!iniDict.ContainsKey(keyvalue[0].Trim()))
                        {
                            iniDict.Add(keyvalue[0].Trim(), keyvalue[1].Trim());
                        }
                        else
                        {
                            //If duplicate element is found, the priority is given to filament parameter value
                            if (option == searchingOption.Filament)
                            {
                                iniDict[keyvalue[0].Trim()] = keyvalue[1].Trim();
                            }
                        }
                    }
                }
                switch (option)
                {
                    case searchingOption.Printer:
                        {
                            if (line == "[printer:" + selection + "]")
                            {
                                found = true;
                                _log.logWrite("Printer found: " + selection);
                            }
                            break;
                        }
                    case searchingOption.Filament:
                        {
                            if (line == "[filament:" + selection + "]")
                            {
                                found = true;
                                _log.logWrite("Filament found: " + selection);
                            }
                            break;
                        }
                    case searchingOption.Print_Profile:
                        {
                            if (line == "[print:" + selection + "]")
                            {
                                found = true;
                                _log.logWrite("Print profile found: " + selection);
                            }
                            break;
                        }
                }
            }

            // Not found event log
            if (found == false)
            {
                switch (option)
                {
                    case searchingOption.Printer:
                        {
                            _log.logWrite("Printer not found: " + selection);
                            break;
                        }
                    case searchingOption.Filament:
                        {
                            _log.logWrite("Filament not found: " + selection);
                            break;
                        }
                    case searchingOption.Print_Profile:
                        {
                            _log.logWrite("Print profile not found: " + selection);
                            break;
                        }
                }
            }
        }
        #endregion
    }
}