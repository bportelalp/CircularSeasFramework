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
using CircularSeasManager.Resources;

namespace CircularSeasManager.ViewModels {
    public class LoginPageViewModel : LoginPageModel {

        /*COMANDOS: En arquitectura MVVM son métodos que se ejecutan en respuesta a una actividad específica
        Normalmente por ejemplo al clicar o pulsar un botón*/
        public Command CmdIniciarSesion { get; set; }
        public Command CmdExpert { get; set; }

        public LoginPageViewModel() {
            /*Se asocia cada comando al método correspondiente(de forma asincrona) y además se indica que
             se ejecutan sólo si Busy es false, es decir, que no haya otro método en curso*/
            CmdIniciarSesion = new Command(async()=>await Login(), () => !Busy);
            CmdConfig = new Command(async () => await GoToSettings(), () => !Busy);
            CmdExpert = new Command(async () => await ExpertMode(), () => !Busy);
            _ = GetSecureCredenciales(); //no es awaited pero no importa, porque no devuelve valor, se añade descarte
        }
        
        //Método asociado al comando iniciar sesión
        private async Task Login() {
            //Crea el objeto con la nueva IP, comprobando antes si lleva la cabecera http://
            Global.PrinterClient = new OctoClient((IPOctoprint.StartsWith("http://") == true) ? IPOctoprint : ("http://" + IPOctoprint));
            Global.ClienteSlice = new SliceClient((IPSlicer.StartsWith("http://") == true) ? IPSlicer : ("http://" + IPSlicer));
            Busy = true;
            var result = await Global.PrinterClient.Login(UserInput, Pass);
            Busy = false;

            if (result) {
                InitMessage = StringResources.Logged;
                if (Rememberme) {
                    try {
                        await SecureStorage.SetAsync("user", UserInput);
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
                var resultado2 = await Global.PrinterClient.GetConexPrinter();
                if (resultado2 != null) {
                    printer = resultado2.options.printerProfiles[0].name;
                }
                Application.Current.MainPage = new NavigationPage(new Views.MainPanel());
            }

            else { 
                if (Global.PrinterClient.ResultRequest == Services.RequestState.Auth) {
                    InitMessage = StringResources.UserPassWrong;
                }
                else if (Global.PrinterClient.ResultRequest == Services.RequestState.NoConnection) {
                    InitMessage = StringResources.ErrorConnection;
                }
            }      
        }

        private async Task ExpertMode() {
            await Browser.OpenAsync(IPOctoprint, BrowserLaunchMode.SystemPreferred);
        }
    }
}
