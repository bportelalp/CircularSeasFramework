using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace CircularSeasWebAPI.Helpers
{
    public class Log
    // Class about a log file to help debugging phase and traceability information storage
    {
        string logPath;
        public Log(IWebHostEnvironment env)
        {
            logPath = env.WebRootPath + "\\" + WebFolder.Logs + "\\Backend-" + DateTime.Today.ToString("yyyy-MM-dd") + ".log";
        }

        /// <summary>
        /// Write a message in the logFile specified in its propierty
        /// </summary>
        /// <param name="message"> Contains the message to be written in a string datatype</param>
        /// <returns></returns>
        public async void logWrite(string message)
        {
            if (logPath == null)
                return;

            StreamWriter sw;
            try
            {
                //If the log file does not exist, then creates it. Append = true stands for adding new lines.
                sw = new StreamWriter(logPath, true, System.Text.Encoding.UTF8);
                
                // Writing current date and message of the event to storage in the log file
                //(nota futura, distinguir mensajes error de mensajes aviso)
                await sw.WriteLineAsync(DateTime.Now + " - " + message);

                Console.WriteLine(DateTime.Now + "-" + message); //Debugging console message
                sw.Close();
            }
            catch(Exception ex)
            {
                return;
            }
        }
    }
}