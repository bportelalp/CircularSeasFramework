using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Net.Http.Json;
using System.Linq;
using System.Threading.Tasks;
using CircularSeasManager.Models;
using Xamarin.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;
using CircularSeas.Models.DTO;

namespace CircularSeasManager.ViewModels
{
    public class OrderViewModel : OrderModel
    {
        public Services.SliceClient SliceClient => DependencyService.Get<Services.SliceClient>();
        public Services.IQrService qrService => DependencyService.Get<Services.IQrService>();
        public HttpClient Http => DependencyService.Get<HttpClient>();

        public Command CmdOrder { get; set; }
        public Command CmdScanSpool { get; set; }
        public Command CmdDiscardSpool { get; set; }
        public OrderViewModel(Guid materialCandidate)
        {
            _materialCandidate = materialCandidate;

            CmdOrder = new Command(async () => await SendOrder(), () => !Busy);
            CmdScanSpool = new Command(async () => await ManageSpool(true), () => !Busy);
            CmdDiscardSpool = new Command(async () => await ManageSpool(false), () => !Busy);


            Materials = new ObservableCollection<CircularSeas.Models.Material>();
            PendingOrders = new ObservableCollection<CircularSeas.Models.Order>();
            _ = GetData();
        }

        public async Task GetData()
        {
            var result = await SliceClient.GetMaterials();
            result.ForEach(r => Materials.Add(r));
            MaterialSelected = Materials
                .Where(m => m.Id == _materialCandidate)
                .FirstOrDefault();



            await GetOrders();
        }

        public async Task SendOrder()
        {
            Busy = true;
            var order = new CircularSeas.Models.Order()
            {
                CreationDate = DateTime.Now,
                MaterialFK = MaterialSelected.Id,
                SpoolQuantity = Amount,
                NodeFK = NodeId,
                ProviderFK = new Guid("F83FEEF7-6278-4335-80CB-798635F9DDED")
            };

            var content = new StringContent(JsonConvert.SerializeObject(order));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await Http.PostAsync("api/management/order/new", content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CircularSeas.Models.Order>();
                await GetOrders();
            }
            Busy = false;
        }

        public async Task GetOrders()
        {
            PendingOrders.Clear();
            var route = QueryHelpers.AddQueryString("api/management/order/list",
                new Dictionary<string, string>()
                {
                    {"status", "0"},
                    {"nodeId", NodeId.ToString() }
                });
            var response = await Http.GetAsync(route);
            var orders = await response.Content.ReadFromJsonAsync<List<CircularSeas.Models.Order>>();
            orders.OrderBy(o => o.CreationDate).ToList().ForEach(o => PendingOrders.Add(o));
        }

        public async Task ManageSpool(bool registration)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                if (registration)
                {
                    var scanned = await qrService.ScanAsync();
                    if (scanned != null)
                    {
                        QrDTO qr = JsonConvert.DeserializeObject<QrDTO>(scanned);

                        var response = await Http.PutAsync($"api/management/order/mark-received/{qr.OrderId}", null);
                        if (response.IsSuccessStatusCode)
                        {
                            await Application.Current.MainPage.DisplayAlert("TodoOk", "Registrado", "Aceptar");
                        }
                    }

                }
                else
                {
                    var scanned = await qrService.ScanAsync();
                    QrDTO qr = JsonConvert.DeserializeObject<QrDTO>(scanned);
                    var response = await Http.PutAsync($"api/management/order/mark-spended/{NodeId}/{qr.MaterialId}/1", null);
                    if (response.IsSuccessStatusCode)
                    {
                        await Application.Current.MainPage.DisplayAlert("TodoOk", "Registrado", "Aceptar");
                    }
                }
            }

            else if (Device.RuntimePlatform == Device.UWP)
            {

            }

        }
    }
}
