using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp.Authenticators;
using RestSharp;
using Plugin.FilePicker;



namespace CircularSeasManager.Services {

    /*ESTE ES EL SERVICIO. Aqui se incorpora objeto y métodos encargados de gestionar los cálculos para pedir
     datos y demás*/
    public enum EstadoRequest {
        Ok,
        SinConexion,
        Auth,
        Ocupado,
        NoExiste,
        SinConexPrinter,
        Otro,
        ExtensionIncorrecta,
    }

    public class OctoCliente {
        private string urlbase;
        private RestClient cliente;
        private FilesJSON.RootObj FilesGuardado = new FilesJSON.RootObj();
        public EstadoRequest ResultRequest;

        /// <summary>Constructor, construye objeto asociado a una URL del servidor </summary>
        /// <param name="urlbase_"> URL base del servicio REST</param>
        public OctoCliente(string urlbase_) {
            urlbase = urlbase_;
            cliente = new RestClient(urlbase);
        }

        /// <summary>Función interna para obtener si la petición fue bien. Si fue mal, guarda algunos de los códigos más comunes en ResultRequest.
        /// Para otros códigos más específicos, se realiza la comprobación en la propia tarea.</summary>
        /// <param name="statuscode">Se le pasa el response.statuscode para hacer a comprobación.</param>
        /// <returns>booleano indicando si todo bien o no</returns>
        internal bool SinErroresHttp(HttpStatusCode statuscode) {
            switch (statuscode) {
                case HttpStatusCode.OK:
                    ResultRequest = EstadoRequest.Ok;
                    return true;
                case HttpStatusCode.NoContent:
                    ResultRequest = EstadoRequest.Ok;
                    return true;
                case HttpStatusCode.Created:
                    ResultRequest = EstadoRequest.Ok;
                    return true;
                case HttpStatusCode.Forbidden:
                    ResultRequest = EstadoRequest.Auth;
                    return false;
                case 0:
                    ResultRequest = EstadoRequest.SinConexion;
                    return false;
                default:
                    ResultRequest = EstadoRequest.Otro;
                    return false;
            }
        }

        /*TAREAS ADMINISTRATIVAS:
         * Se encargan de tareas administrativas:
         * - login. Iniciar sesión con el usuario y contraseña que se pasa por parámetro.
         * - logout. Cerrar sesión con dicho usuario.
         * - GetVersion. Versión actual de Octoprint.
         * - GetConexPrinter. Revisar el estado actual de conexión con la impresora
         * - PostConexPrinter. Conectarse a la impresora por defecto.
         */

        ///<summary>Establece conexión autenticada con la REST API de Octoprint de forma asíncrona.</summary>
        public async Task<bool> login(string user, string password) {
            //Activa contenedor de cookies y establece los datos de autenticación
            cliente.CookieContainer = new CookieContainer();
            //Genera body con los datos de inicio de sesión.
            var credencial = new LoginJSON.RootObj();
            credencial.user = user;
            credencial.pass = password;

            //Realiza la petición de login, con esta petición ya se guarda la primera cookie para sucesivas
            var request = new RestRequest("/api/login", Method.POST);
            request.AddJsonBody(credencial);

            var response = await cliente.ExecuteAsync(request);

            if (SinErroresHttp(response.StatusCode)) { return true; }
            else { return false; }

        }

        /// <summary>Cierra sesión en el servidor.</summary>
        /// <returns></returns>
        public async Task<bool> logout() {
            //Petición de cerrar la sesión.
            var request = new RestRequest("/api/logout", Method.POST);
            var response = await cliente.ExecuteAsync(request);

            if (SinErroresHttp(response.StatusCode)) { return true; }
            else { return false; }

        }

        ///<summary>Obtener versión de octoprint </summary>
        public async Task<string> GetVersion() {
            // Realiza petición de la versión.
            var request = new RestRequest("/api/version", Method.GET);
            var response = await cliente.ExecuteAsync(request);
            //Importa el JSON como objeto

            if (SinErroresHttp(response.StatusCode)) {
                var version = JsonConvert.DeserializeObject<VersionJSON.RootObj>(response.Content);
                return version.text;
            }
            else { return null; }

        }

        ///<summary>Devuelve información de conexión con impresora</summary>
        public async Task<ConexJSON.RootObj> GetConexPrinter() {

            var request = new RestRequest("/api/connection", Method.GET);
            var response = await cliente.ExecuteAsync(request);

            if (SinErroresHttp(response.StatusCode)) {
                var conexion = JsonConvert.DeserializeObject<ConexJSON.RootObj>(response.Content);
                return conexion;
            }
            else { return null; }
        }

        /// <summary> Envía a la impresora orden de conectarse </summary>
        /// <param name="ordenConexion"> True: Encender, False: apagar</param>
        /// <returns></returns>
        public async Task<bool> PostConexPrinter(bool ordenConexion, string port = "/dev/ttyACM0", int baudrate = 250000, string printerProfile = "_default") {
            var request = new RestRequest("/api/connection", Method.POST, DataFormat.Json);

            //Prepara los parámetros en función de si se pide encender o apagar
            var peticion = new ConexRequestJSON.RootObj();
            if (ordenConexion) {
                //Conexión típica.
                peticion.command = "connect";
                peticion.port = port;
                peticion.baudrate = baudrate;
                peticion.printerProfile = printerProfile;
            }
            else {
                //Sólo orden desconectar.
                peticion.command = "disconnect";
            }

            //Añade cuerpo y ejecuta la petición
            request.AddJsonBody(peticion);
            var response = await cliente.ExecuteAsync(request);

            if (SinErroresHttp(response.StatusCode)) {
                return true;
            }
            else { return false; }

        }

        /*TAREAS DE FICHEROS:
        * Se encargan de tareas relacionadas con la gestión de los archivos maquina en memoria
        * - GetFiles. Devuelve una lista de los archivos en memoria para imprimir.
        * - DeleteFiles. Elimina el archivo pasado por parámetro
        */

        /// <summary> Obtiene una lista con los nombres de los archivos almacenados en memoria local de la raspberry </summary>
        /// <returns></returns>
        public async Task<List<string>> GetFiles() {
            //Realizar la petición
            var request = new RestRequest("/api/files/local", Method.GET);
            var response = await cliente.ExecuteAsync(request);
            //Construir lista con todos los nombres de los archivos encontrados en local, directorio raíz.

            

            if (SinErroresHttp(response.StatusCode)) {
                var ficheros = JsonConvert.DeserializeObject<FilesJSON.RootObj>(response.Content);
                FilesGuardado = ficheros;
                
                /*--------COSAS PRUEBAS-----------*/
                /*var req = new RestRequest(ficheros.files[0].refs.download, Method.GET);
                var respons = await cliente.ExecuteAsync(req);
                byte[] bites = Encoding.UTF8.GetBytes(respons.Content);

                //Cómo sacar el filename del header
                string filename = "";
                foreach(var item in respons.Headers) {
                    if (item.Name == "Content-Disposition") {
                        filename = (string)item.Value;
                        filename.IndexOf("filename=\"");
                        filename = filename.Substring(filename.IndexOf("UTF-8\'\'") + 7);

                    }
                }
                await this.UploadFile(bites, filename, false);*/


                /*---------COSAS PRUEBAS*/



                //Crear lista de ficheros
                List<string> listafich = new List<string>();
                for (int i = 0; i < ficheros.files.Length; i++) {
                    if (ficheros.files[i].type == "machinecode") {
                        listafich.Add(ficheros.files[i].name);
                    }
                }

                return listafich;
            }
            else { return null; }
        }

        /// <summary>Elimina el archivo pasado en path (si es local, sirve sólo con el nombre</summary>
        /// <param name="path">Ruta del archivo (o nombre si está en local</param>
        /// <returns>Resultado de la petición</returns>
        public async Task<bool> DeleteFile(string path) {
            //Ejecuta petición
            var request = new RestRequest("/api/files/local/" + path, Method.DELETE);
            var response = await cliente.ExecuteAsync(request);
            if (SinErroresHttp(response.StatusCode)) {
                return true; //Borrado correcto
            }
            else {
                if (response.StatusCode == HttpStatusCode.Conflict) {
                    //Se produce porque ese archivo está siendo procesado por la impresora
                    ResultRequest = EstadoRequest.Ocupado;
                    return false;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound) {
                    //Porque no existe ya en la base de datos
                    ResultRequest = EstadoRequest.NoExiste;
                    return false;
                }
                else {//Cualquer otro caso (auth, conexión...
                    return false;
                }
            }
        }

        /// <summary>
        /// Envía al servicio Rest un archivo con extensión .gcode, para imprimirlo por Octoprint
        /// </summary>
        /// <param name="gcod">Archivo a enviar, de tipo FileData (Plugin Fileicker)</param>
        /// <param name="imprimir">Orden de impresión (true imprimir, false no imprimir)</param>
        /// <returns></returns>
        public async Task<bool> UploadFile(byte[] gcod, string name, bool imprimir) {
            var request = new RestRequest("/api/files/local", Method.POST);

            //FALTA POR IMPLEMENTAR TRY CATCH PARA EL FILEPICKER Y ADEMÁS LA GESTION DE ERRORES POR COMUNICACIONES.
            if (imprimir) {
                //si se desea imprimir, se añaden estos parámetros.
                request.AddParameter("select", "true");
                request.AddParameter("print", "true");
            }
            
            request.AddFileBytes("file", gcod, name, "application/octet-stream");
            var response = await cliente.ExecuteAsync(request);
            if (SinErroresHttp(response.StatusCode)) {
                return true;
            }
            else {
                if (response.StatusCode == HttpStatusCode.BadRequest) {
                    //Este bad request en realidad ocurre por enviar un archivo con extensión incorrecta, se lo indico
                    //No debería pasar por el if inicial pero igualmente se lo considero
                    ResultRequest = EstadoRequest.ExtensionIncorrecta;
                    return false;
                }
                else {
                    //Cualquier otro caso, como conexión, ya se lo asignaría el sinerroreshttp
                    return false;
                }
            }

        }

        /*TAREAS DE PROCESO:
         * Se encargan de tareas directamente relacionadas con el proceso de imprimir
         * - GetCurrentJob. Información acerca del proceso actual de impresión
         * - PostImprimir. Envía un fichero para imprimir.
         * - GetPrinterState. Básicamente para obtener las temperaturas de hotend y bed.
         */

        ///<summary>Devuelve información sobre el trabajo en curso</summary>
        public async Task<JobJSON.RootObj> GetCurrentjob() {
            var request = new RestRequest("/api/job", Method.GET);
            var response = await cliente.ExecuteAsync(request);

            if (SinErroresHttp(response.StatusCode)) {
                var trabajo = JsonConvert.DeserializeObject<JobJSON.RootObj>(response.Content);
                return trabajo;
            }
            else { return null; }
        }

        /// <summary>Envía una orden de impresión</summary>
        /// <param name="name">Nombre del archivo que se desea imprimir</param>
        /// <returns>Bool que indica si el resultado fue bueno, si no, el estado se almacena en ResultRequest.</returns>
        public async Task<bool> PostImprimir(string name) {
            string path = "";
            for (int i = 0; i < FilesGuardado.files.Length; i++) {
                if (FilesGuardado.files[i].name == name) {
                    path = "local/" + FilesGuardado.files[i].path;
                    break;
                }
            }
            var request = new RestRequest("/api/files/" + path, Method.POST);
            //Ejecuta comando seleccionar y activa la impresión inmediata.
            var JSONenvio = new FileCommandJSON.RootObj();
            JSONenvio.command = "select";
            JSONenvio.print = true;
            request.AddJsonBody(JSONenvio);

            var response = await cliente.ExecuteAsync(request);
            if (SinErroresHttp(response.StatusCode)) {
                return true;
            }
            else {
                //Aquí podría ser adicionalmente 404 not found si no se encuentra el fichero o 409 Conflict si la impresora no está operativa.
                if (response.StatusCode == HttpStatusCode.NotFound) {
                    ResultRequest = EstadoRequest.NoExiste;
                    return false;
                }
                if (response.StatusCode == HttpStatusCode.Conflict) {
                    //Comprueba el estado.
                    var trabajoActual = await GetCurrentjob();
                    if (trabajoActual == null) {
                        return false;
                    }
                    else {
                        if (trabajoActual.state == "Printing") {
                            ResultRequest = EstadoRequest.Ocupado;
                            return false;
                        }
                        if (trabajoActual.state == "Offline") {
                            ResultRequest = EstadoRequest.SinConexPrinter;
                            return false;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>Recupera información sobre el estado de la impresora</summary>
        /// <returns>JSON con las temperaturas actual/objetivo de hotend y bed</returns>
        public async Task<PrinterStateJSON.RootObj> GetPrinterState() {
            var request = new RestRequest("/api/printer", Method.GET);
            request.AddParameter("exclude", "sd, state");
            request.AddParameter("history", "false");

            var response = await cliente.ExecuteAsync(request);
            if (SinErroresHttp(response.StatusCode)) {
                var printerstatus = JsonConvert.DeserializeObject<PrinterStateJSON.RootObj>(response.Content);
                return printerstatus;
            }
            else {
                return null;
            }
        }
        /// <summary> Envía un comando a la impresora, permitiendo iniciar, para o cancelar trabajos de impresión.</summary>
        /// <param name="comando"> Cancelar enviar "cancel", Pausar enviar "pause"</param>
        /// <returns></returns>
        public async Task<bool> PostComandoJob(string comando) {
            var request = new RestRequest("/api/job", Method.POST);

            var JSONenvio = new FileCommandJSON.RootObj();
            JSONenvio.command = comando;
            if (comando == "pause") {
                //Enviando toggle, si está pausado reanuda, si está funcionando, pausa.
                JSONenvio.action = "toggle";
            }
            request.AddJsonBody(JSONenvio);

            var response = await cliente.ExecuteAsync(request);
            if (SinErroresHttp(response.StatusCode)) {
                return true;
            }
            else {
                if (response.StatusCode == HttpStatusCode.Conflict) {
                    //Esto se produce porque ya está parada o no tiene sentido, devuelve igualmente true
                    ResultRequest = EstadoRequest.Ok;
                    return true;
                }
                else {
                    return false;
                }
            }
        }

    }

    //Objeto del JSON devuelto por currentJob()
    namespace JobJSON {

        public class RootObj {
            //[JsonProperty("job")]
            public Job job { get; set; }
            public Progress progress { get; set; }
            public string state { get; set; }
        }

        public class Job {
            public File file { get; set; }
            public double? estimatedPrintTime { get; set; }
            public Filament filament { get; set; }
        }

        public class File {
            public string name { get; set; }
            public string origin { get; set; }
            public int? size { get; set; }
            public int? date { get; set; }
        }

        public class Filament {
            public int? length { get; set; }
            public float? volume { get; set; }
        }

        public class Progress {
            public float? completion { get; set; }
            public int? filepos { get; set; }
            public int? printTime { get; set; }
            public int? printTimeLeft { get; set; }
        }

    }

    //JSON para pedir la conexión
    namespace ConexRequestJSON {

        public class RootObj {
            public string command { get; set; }
            public string port { get; set; }
            public int baudrate { get; set; }
            public string printerProfile { get; set; }
            public bool save { get; set; }
            public bool autoconnect { get; set; }
        }

    }

    //JSON devuelto por /api/connection
    namespace ConexJSON {

        public class RootObj {
            public Current current { get; set; }
            public Options options { get; set; }
        }

        public class Current {
            public string state { get; set; }
            public string port { get; set; }
            public int? baudrate { get; set; }
            public string printerProfile { get; set; }
        }

        public class Options {
            public string[] ports { get; set; }
            public int[] baudrates { get; set; }
            public Printerprofile[] printerProfiles { get; set; }
            public string portPreference { get; set; }
            public int? baudratePreference { get; set; }
            public string printerProfilePreference { get; set; }
            public bool autoconnect { get; set; }
        }

        public class Printerprofile {
            public string name { get; set; }
            public string id { get; set; }
        }

    }

    //JSON para iniciar sesión
    namespace LoginJSON {
        public class RootObj {
            public string user { get; set; }
            public string pass { get; set; }
        }
    }

    //JSON devuelto por /api/files
    namespace FilesJSON {

        public class RootObj {
            public File[] files { get; set; }
            public string free { get; set; }
        }

        public class File {
            public string name { get; set; }
            public string path { get; set; }
            public string type { get; set; }
            public string[] typePath { get; set; }
            public string hash { get; set; }
            public int size { get; set; }
            public int date { get; set; }
            public string origin { get; set; }
            public Refs refs { get; set; }
            public Gcodeanalysis gcodeAnalysis { get; set; }
            public Print print { get; set; }
            public Child[] children { get; set; }
        }

        public class Refs {
            public string resource { get; set; }
            public string download { get; set; }
        }

        public class Gcodeanalysis {
            public float estimatedPrintTime { get; set; }
            public Filament filament { get; set; }
        }

        public class Filament {
            public int length { get; set; }
            public float volume { get; set; }
        }

        public class Print {
            public int failure { get; set; }
            public int success { get; set; }
            public Last last { get; set; }
        }

        public class Last {
            public int date { get; set; }
            public bool success { get; set; }
        }

        public class Child {
            public string name { get; set; }
            public string path { get; set; }
            public string type { get; set; }
            public string[] typePath { get; set; }
            public string hash { get; set; }
            public int size { get; set; }
            public int date { get; set; }
            public string origin { get; set; }
            public Refs1 refs { get; set; }
            public Gcodeanalysis1 gcodeAnalysis { get; set; }
            public Print1 print { get; set; }
        }

        public class Refs1 {
            public string resource { get; set; }
            public string download { get; set; }
        }

        public class Gcodeanalysis1 {
            public int estimatedPrintTime { get; set; }
            public Filament1 filament { get; set; }
        }

        public class Filament1 {
            public int length { get; set; }
            public float volume { get; set; }
        }

        public class Print1 {
            public int failure { get; set; }
            public int success { get; set; }
            public Last1 last { get; set; }
        }

        public class Last1 {
            public int date { get; set; }
            public bool success { get; set; }
        }

    }

    //JSON devuelto por /api/version
    namespace VersionJSON {

        public class RootObj {
            public string api { get; set; }
            public string server { get; set; }
            public string text { get; set; }
        }

    }

    //JSON para file command
    namespace FileCommandJSON {
        public class RootObj {
            public string command { get; set; }
            public bool print { get; set; }
            public string action { get; set; }
        }
    }

    //JSON para informacion impresora
    namespace PrinterStateJSON {

        public class RootObj {
            public Temperature temperature { get; set; }
        }

        public class Temperature {
            public Tool0 tool0 { get; set; }
            public Bed bed { get; set; }
        }

        public class Tool0 {
            public float actual { get; set; }
            public float target { get; set; }
            public int offset { get; set; }
        }

        public class Bed {
            public float actual { get; set; }
            public float target { get; set; }
            public int offset { get; set; }
        }

    }
}
