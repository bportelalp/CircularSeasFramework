using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircularSeasWebAPI.Helpers {
    public class Tools {

        // Service access
        private readonly Log _log;
        private readonly IWebHostEnvironment _env;
        private readonly AppSettings _appSettings;

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
        public Tools(Log log, IOptions<AppSettings> appSettings, IWebHostEnvironment env) {
            this._log = log;
            this._env = env;
            _appSettings = appSettings.Value;
        }
        #region "Methods"
        /// <summary>
        /// Application of the TOPSIS method
        /// </summary>
        /// <param name="criteria"> MxN matrix (Materials_x_Criteria) that includes the mark for material i on criterion j in each term Xij </param>
        /// <param name="evaluation"> Vector of N elements (criteria) including importance (evaluation/mark) requested by the user for the j criterion </param>
        /// <param name="impact"> Distinguish if the higher the better (true), or the higher the worse (false) </param>
        /// <returns> Matrix that contains the ranked performance of the available materials </returns>
        public double[] TOPSIS(double[,] criteria, double[] evaluation, bool[] impact) {

            //Dimension parameter determination
            int n_mat = criteria.GetLength(0);
            int n_crit = criteria.GetLength(1);
            int n_eval = evaluation.Length;

            /*STEP 1: Normalization of the decision matrix and evaluation because the values may not be defined by the domain itself. It is normalized as Nij = Xij / ((Sum, j = 1 to m) of (X_ij) ^ 2)) */

            //Getting the common denominator of all the elements of each column (summation)
            double[] den_normdecision = new double[n_crit];
            for (int j = 0; j < n_crit; j++) {
                double quadraticSum = 0;
                for (int i = 0; i < n_mat; i++) {
                    quadraticSum += Math.Pow(criteria[i, j], 2);
                }
                den_normdecision[j] = Math.Sqrt(quadraticSum);
            }

            // Normalization of the criteria matrix. For each element X_ij, it is normalized with respect to the column (criteria). 
            double[,] crit_norm = new double[n_mat, n_crit];
            for (int i = 0; i < n_mat; i++) {
                for (int j = 0; j < n_crit; j++) {
                    crit_norm[i, j] = criteria[i, j] / den_normdecision[j];
                }
            }

            // Decision matrix normalization, so that the sum of weights is 1. Wn_j = W_j / (summation W_j) 
            double sumaeval = 0;
            for (int i = 0; i < n_eval; i++) {
                sumaeval += evaluation[i];
            }
            double[] eval_norm = new double[n_eval];
            for (int i = 0; i < n_eval; i++) {
                eval_norm[i] = evaluation[i] / sumaeval;
            }

            /*STEP 2: Construction of weighted normalized decision matrix (criteria ponderation matrix). V_ij = Wn_j x N_ij */
            double[,] crit_pond = new double[n_mat, n_crit];
            for (int i = 0; i < n_mat; i++) {
                for (int j = 0; j < n_crit; j++) {
                    crit_pond[i, j] = crit_norm[i, j] * eval_norm[j];
                }
            }

            /*STEP 3: Determine best and worst solution. If the criteria are of benefit, for each criterion A+ = (max V_ij) and A- = (min V_ij), if they are cost then * A+ = (min V_ij) and A- = (max V_ij) */ 

            double[] Aplus = new double[n_crit];
            double[] Aminus = new double[n_crit];
            for (int j = 0; j < n_crit; j++) {
                double max = crit_pond[0, j]; //First element selection
                double min = crit_pond[0, j]; //First element selection
                for (int i = 1; i < n_mat; i++) {
                    if (crit_pond[i, j] > max) {
                        max = crit_pond[i, j];
                    }
                    if (crit_pond[i, j] < min) {
                        min = crit_pond[i, j];
                    }
                }
                // Depending on the impact, it is assigned as positive or negative 
                if (impact[j]) {
                    Aplus[j] = max;
                    Aminus[j] = min;
                }
                else {
                    Aplus[j] = min;
                    Aminus[j] = max;
                }
                
            }

            /*STEP 4: Calculation of the distance measures of each Alternative(Material) to the positive ideal solution and the negative ideal solution. D+ = ROOT((Sum of j = 1 to n (V_ij-A_j +) ^ 2)) and D+ = ROOT((Sum of j = 1 to n (V_ij-A_j -) ^ 2)) */
            double[] Dplus = new double[n_mat];
            double[] Dminus = new double[n_mat];
            for (int i = 0; i < n_mat; i++) {
                double rowplus = 0;
                double rowminus = 0;
                for (int j = 0; j < n_crit; j++) {
                    rowplus += Math.Pow((crit_pond[i, j] - Dplus[j]), 2);
                    rowminus += Math.Pow((crit_pond[i, j] - Dminus[j]), 2);
                }
                Dplus[i] = Math.Sqrt(rowplus);
                Dminus[i] = Math.Sqrt(rowminus);
            }

            /*STEP 5: Performance calculus: Relative proximity to the ideal solution. It is calculated as Ri = di- /(di+ + di-). The performance returns values for each alternative that the higher its value, the better the alternative (material) is, according to the given criteria. */ 
            double[] perform = new double[n_mat];
            for (int i = 0; i < n_mat; i++) {
                perform[i] = Dminus[i] / (Dplus[i] + Dminus[i]);
            }

            return perform;
        }

        /// <summary>
      /// Creation of the PrusaSlicer configuration file to print
      /// </summary>
      /// <param name="_printer"> Name of the printer selected </param>
      /// <param name="_filament"> Name of the material selected </param>
      /// <param name="_profile"> Name of the print profile (quality) selected </param>
      /// <param name="_params"> Dictionary that contains all the specific parameters to overwrite the ones in the .ini files </param>
      /// <returns> The name of the file generated (inside this function) that contains the configuration selected</returns>
        public string ConfigFileCreator(string _printer, string _filament, string _profile, Dictionary<string, string> _params) {
            
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
            foreach (KeyValuePair<string, string> parameter in _params){
               if (iniDict.ContainsKey(parameter.Key.Trim())){
                    iniDict[parameter.Key]=parameter.Value;
                }
               else
                {
                    iniDict.Add(parameter.Key, parameter.Value);
                }
            }

            // Load all keys into a list, value separated by = 
            foreach (KeyValuePair<string, string> propiedad in iniDict) {
                iniList.Add(propiedad.Key + " = " + propiedad.Value);
            }

            // Write each line to a file
            System.IO.File.WriteAllLines(this.GetWebPath(WebFolder.INI) + iniName, iniList);
            return iniName;
        }

        public string GetWebPath(WebFolder folder) {
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
            string[] bundle = System.IO.File.ReadAllLines(this.GetWebPath(WebFolder.Bundle)+ fileName);

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
             if (found == false) {
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

        /// <summary>
        /// Function that executes the Slicing / CAM process in PrusaSlicer via CLI. 
        /// </summary>
        /// <param name="_attributes"> Contains all the attributes to execute the CAM process with the loading of the selected settings.  </param>
        /// <returns> A string is returned with the information of the error happened during the process; null if not </returns>
        public string ExecuteCommand(string _attributes) {
            //Indicamos que deseamos inicializar el proceso cmd.exe junto a un comando de arranque. 
            string command = "\"" + _appSettings.prusaSlicerPath + "\" ";

            // Indication to close the cmd process when the assigned task is finished.             
            System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo(command);
            procStartInfo.Arguments = _attributes;
            // Indication that the process output be redirected in a Stream.
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.UseShellExecute = false;
            // Indicates that the process does not display a black screen (The process runs in the background) 
            procStartInfo.CreateNoWindow = false;
            // Process initilization
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();

            // Get the output/error of the Console (Stream) and return a text string 
            var outputInfo = proc.StandardOutput;
            var errorInfo = proc.StandardError;
            proc.WaitForExit();

            string resultInfo = outputInfo.ReadToEnd();
            string resultError = errorInfo.ReadToEnd();
            _log.logWrite("\n" + resultInfo);
            _log.logWrite("\n" + resultError);
            //
            //Muestra en pantalla la salida del Comando
            proc.Kill();
            if (String.IsNullOrEmpty(resultError)) {
                return null;
            }
            else { return resultError; }

        }
        #endregion
    }
}