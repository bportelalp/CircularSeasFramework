using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using RestSharp;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CircularSeasManager.Models;
using CircularSeas;

namespace CircularSeasManager.Services {
    public class SliceCliente {
        private string urlbase;
        private RestClient cliente;
        public HttpStatusCode resultRequest;

        public SliceCliente(string _urlbase) {
            urlbase = _urlbase;
            cliente = new RestClient(urlbase);
        }
        /// <summary>
        /// Obter os datos de Materiais e calidades dispoñibles para facer a conversión
        /// </summary>
        /// <param name="IDprinter">Identificador da impresora</param>
        /// <returns>Obxeto ca información do JSON deserializada</returns>
        public async Task<CircularSeas.Models.DTO.DataDTO> GetDatos(string IDprinter) {
            //Solicitude dos datos ao servizo na nube, para o ID da impresora fixado
            var request = new RestRequest("/circularseas/Printer/" + IDprinter, Method.GET);
            //Espera recepción
            var response = await cliente.ExecuteAsync(request);
            //Comproba resultado e devolve en consonancia
            resultRequest = response.StatusCode;
            if (response.StatusCode == HttpStatusCode.OK) { //Non se estableceu conexion
                return JsonConvert.DeserializeObject<CircularSeas.Models.DTO.DataDTO>(response.Content);
            }
            else {
                return null;
            }
        }
        /// <summary>
        /// Solicitar unha conversion de STL->GCODE no servizo na nube
        /// </summary>
        /// <param name="_STL">Ficheiro STL</param>
        /// <param name="_Printer">Identificador da impresora</param>
        /// <param name="_Material">Identificador do material</param>
        /// <param name="_Quality">Identificador da calidade</param>
        /// <param name="_Support">Habilitar ou non os soportes</param>
        /// <param name="octoCliente">Instancia que se comunica co servizo local</param>
        /// <returns></returns>
        public async Task<Tuple<string,byte[]>> PostSTL(Plugin.FilePicker.Abstractions.FileData _STL, string _Printer, string _Material, string _Quality, bool _Support) {
            //Implementación de envío del stl para el laminado
            var request = new RestRequest("/circularseas/upload", Method.POST);
            //Engadir os parámetros seleccionados para a configuración
            request.AddQueryParameter("printer", _Printer);
            request.AddQueryParameter("material", _Material);
            request.AddQueryParameter("quality", _Quality);
            request.AddQueryParameter("support", _Support.ToString());
            //Engadir o ficheiro entrante
            request.AddFileBytes("file", _STL.DataArray, _STL.FileName, "application/octet-stream");
            //Esperar pola resposta e recollela
            var response = await cliente.ExecuteAsync(request);
            resultRequest = response.StatusCode;
            if (resultRequest == HttpStatusCode.OK) {
                byte[] bites = Encoding.UTF8.GetBytes(response.Content);
                var nomeGCODE = _STL.FileName.Split(new char[] { '.' })[0] + "_" + _Material + "_" + _Quality + ".gcode";
                //Reenviar ao servizo local
                //await octoCliente.UploadFile(bites, nomeGCODE, false);
                //Podria ponrse response.RawBytes e eliminar a liña anterior
                return new Tuple<string, byte[]>(nomeGCODE, bites);
            }
            else {
                return new Tuple<string, byte[]>(null, null);
            }


        }

        
    }

    
    
}
