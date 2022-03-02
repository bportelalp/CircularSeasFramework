using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CircularSeasManager.Models;
using CircularSeasManager.Resources;
using Xamarin.Forms;

namespace CircularSeasManager.ViewModels
{
    public class ConfigViewModel : ConfigModel, IDisposable
    {
        public HttpClient Http => DependencyService.Get<HttpClient>();
        public Command CmdOrders { get; set; }

        public ConfigViewModel()
        {

            //CmdOrders = new Command(async () => await CheckPing(), () => !Busy);
            Nodes = new System.Collections.ObjectModel.ObservableCollection<CircularSeas.Models.Node>();
            //Initialize as the default
            if (!string.IsNullOrEmpty(NodeName) && NodeId != default(Guid))
            {


                NodeSelected = new CircularSeas.Models.Node() { Id = new Guid(NodeId.ToString()), Name = NodeName.ToString() };

                Nodes.Add(NodeSelected);
            }

            timerCloud = new System.Timers.Timer(1000);
            timerCloud.Elapsed += CheckCloudConnection;
            timerCloud.AutoReset = false;

            timerOctoprint = new System.Timers.Timer(1000);
            timerOctoprint.Elapsed += CheckOprintConnection;
            timerOctoprint.AutoReset = false;

            timerCloud.Start();
            timerOctoprint.Start();
        }

        public string IpIntermediateCloud
        {
            get => IPSlicer;
            set
            {
                if (IPSlicer != value)
                {
                    IPSlicer = value;
                    timerCloud.Stop();
                    timerCloud.Start();
                }
            }
        }

        public string IpIntermediateOprint
        {
            get => IPOctoprint;
            set
            {
                if (IPOctoprint != value)
                {
                    IPOctoprint = value;
                    timerOctoprint.Stop();
                    timerOctoprint.Start();
                }
            }
        }
        private async void CheckOprintConnection(Object source, ElapsedEventArgs e)
        {
            try
            {
                Busy = true;
                //Ping System class is not implemented in UWP, son try conection to some API route with low timeout
                using (var ping = new HttpClient())
                {
                    ping.Timeout = TimeSpan.FromMilliseconds(500);
                    ping.BaseAddress = new Uri(IPOctoprint);
                    var request = await ping.GetAsync("api/version");
                    if (request.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        TryOprintMessage = StringResources.IpOctoprintSucess;
                    }
                    else
                        TryOprintMessage = StringResources.IpOctoprintFail;
                }
            }
            catch (Exception ex)
            {
                TryOprintMessage = StringResources.IpOctoprintFail;
            }
            finally
            {
                Busy = false;
            }
        }
        private async void CheckCloudConnection(Object source, ElapsedEventArgs e)
        {
            try
            {
                Busy = true;
                //Ping System class is not implemented in UWP, son try conection to some API route with low timeout
                using (var ping = new HttpClient())
                {
                    ping.Timeout = TimeSpan.FromMilliseconds(500);
                    ping.BaseAddress = new Uri(IPSlicer);
                    var request = await ping.GetAsync("api/management/connection");
                    if (request.IsSuccessStatusCode)
                    {
                        TryCloudMessage = StringResources.IpCloudSucess;
                        GetNodes();
                    }
                    else
                            TryCloudMessage = StringResources.IpCloudFail;
                }
            }
            catch (Exception ex)
            {
                    TryCloudMessage = StringResources.IpCloudFail;
            }
            finally
            {
                Busy = false;
            }
        }

        private async Task GetNodes()
        {
            try
            {
                using (var provisionalClient = new HttpClient())
                {
                    provisionalClient.Timeout = TimeSpan.FromSeconds(3);
                    provisionalClient.BaseAddress = new Uri(IPSlicer);
                    var response = await provisionalClient.GetAsync("api/management/nodes/list");
                    var nodesList = await response.Content.ReadFromJsonAsync<List<CircularSeas.Models.Node>>();
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Nodes.Clear();
                        nodesList.ForEach(n => Nodes.Add(n));
                        var nodeOld = Nodes.Where(n => n.Id == NodeId).FirstOrDefault();
                        NodeSelected = nodeOld;
                    });

                }
            }
            catch (Exception ex)
            {
                //Ignore
            }


        }

        void IDisposable.Dispose()
        {
            timerCloud.Stop();
            timerCloud.Dispose();
        }
    }
}
