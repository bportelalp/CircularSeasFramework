using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CircularSeasManager.Models;
using CircularSeasManager.Services;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace CircularSeasManager.ViewModels {
    public class LoginPageViewModel : PaginaLoginModel {

        /*COMANDOS: En arquitectura MVVM son métodos que se ejecutan en respuesta a una actividad específica
        Normalmente por ejemplo al clicar o pulsar un botón*/
        public Command CmdIniciarSesion { get; set; }
        public Command CmdExpert { get; set; }

        public LoginPageViewModel() {
            /*Se asocia cada comando al método correspondiente(de forma asincrona) y además se indica que
             se ejecutan sólo si ocupado es false, es decir, que no haya otro método en curso*/
            CmdIniciarSesion = new Command(async()=>await IniciarSesion(), () => !Ocupado);
            CmdConfig = new Command(async () => await IrAConfig(), () => !Ocupado);
            CmdExpert = new Command(async () => await ModoExperto(), () => !Ocupado);
            _ = GetSecureCredenciales(); //no es awaited pero no importa, porque no devuelve valor, se añade descarte
        }
        
        //Método asociado al comando iniciar sesión
        private async Task IniciarSesion() {
            //Crea el objeto con la nueva IP, comprobando antes si lleva la cabecera http://
            Global.ClientePrint = new OctoCliente((IPOctoprint.StartsWith("http://") == true) ? IPOctoprint : ("http://" + IPOctoprint));
            Global.ClienteSlice = new SliceCliente((IPSlicer.StartsWith("http://") == true) ? IPSlicer : ("http://" + IPSlicer));
            Ocupado = true;
            var resultado = await Global.ClientePrint.login(Usuario, Pass);
            Ocupado = false;

            if (resultado) {
                MensajeInicio = "Sesión Iniciada";
                if (Recordarme) {
                    try {
                        await SecureStorage.SetAsync("user", Usuario);
                        await SecureStorage.SetAsync("password", Pass);
                    }
                    catch (Exception ex) {
                        //Problemas con el usuario
                    }
                }
                else {
                    SecureStorage.RemoveAll();
                }

                //Obtener Printer REVISION
                var resultado2 = await Global.ClientePrint.GetConexPrinter();
                if (resultado2 != null) {
                    printer = resultado2.options.printerProfiles[0].name;
                }
                Application.Current.MainPage = new NavigationPage(new Views.MainPanel());
            }

            else { 
                if (Global.ClientePrint.ResultRequest == Services.EstadoRequest.Auth) {
                    MensajeInicio = "Usuario o contraseña incorrectos";
                }
                else if (Global.ClientePrint.ResultRequest == Services.EstadoRequest.SinConexion) {
                    MensajeInicio = "Error de conexión. Asegúrese de estar conectado a la misma red y que el servidor está funcionando";
                }
            }      
        }

        private async Task ModoExperto() {
            await Browser.OpenAsync(IPOctoprint, BrowserLaunchMode.SystemPreferred);
        }
    }
}
