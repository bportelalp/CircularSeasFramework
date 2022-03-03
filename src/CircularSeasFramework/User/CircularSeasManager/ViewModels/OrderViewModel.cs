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
using CircularSeasManager.Resources;

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
            await GetMaterials();
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

        public async Task GetMaterials()
        {
            var query = QueryHelpers.AddQueryString("api/management/materials", new Dictionary<string, string>
            {
                {"includeProperties", "false" },
                {"forUsers", "true" },
                {"nodeStock", NodeId.ToString()}
            });

            var response = await Http.GetAsync(query);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<CircularSeas.Models.Material>>();
                Materials.Clear();
                result.ForEach(r => Materials.Add(r));
            }
            
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
                        var orderLocated = PendingOrders.Where(po => po.Id == qr.OrderId).FirstOrDefault();
                        await this.ConfirmOrder(orderLocated);
                    }

                }
                else
                {
                    var scanned = await qrService.ScanAsync();
                    QrDTO qr = JsonConvert.DeserializeObject<QrDTO>(scanned);
                    await this.MarkSpoolSpend(qr.MaterialId, qr.MaterialName);
                }
            }

            else if (Device.RuntimePlatform == Device.UWP)
            {
                if (registration)
                {
                    if (OrderSelected == null)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                                    AlertResources.WarningHeader,
                                    AlertResources.UwpOrder,
                                    AlertResources.Accept);
                    }
                    else
                        await this.ConfirmOrder(OrderSelected);
                }
                else
                {
                    if(MaterialStocked == null)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                                    AlertResources.WarningHeader,
                                    AlertResources.UwpSpoolStock,
                                    AlertResources.Accept);
                    }
                    else
                        await this.MarkSpoolSpend(MaterialStocked.Id, MaterialStocked.Name);
                }
            }

        }

        private async Task ConfirmOrder(CircularSeas.Models.Order order)
        {
            var decision = await Application.Current.MainPage.DisplayAlert(
                            AlertResources.Confirmation,
                            $"{AlertResources.SureConfirmOrders}\n\r " +
                            $"{AlertResources.OrderID}: {order.Id}\n\r " +
                            $"{AlertResources.Material}: {order.Material.Name}\n\r " +
                            $"{AlertResources.Quantity}: {order.SpoolQuantity}",
                            AlertResources.Yes,
                            AlertResources.No);
            if (decision)
            {
                var response = await Http.PutAsync($"api/management/order/mark-received/{order.Id}", null);
                if (response.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        AlertResources.Confirmation,
                        AlertResources.OrderRegistered,
                        AlertResources.Accept);
                    _ = GetOrders();
                    _ = GetMaterials();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        AlertResources.Confirmation,
                        AlertResources.Error,
                        AlertResources.Accept);
                }
            }
        }

        private async Task MarkSpoolSpend(Guid materialId, string materialName)
        {
            var decision = await Application.Current.MainPage.DisplayAlert(
                            AlertResources.Confirmation,
                            $"{AlertResources.SpendOneSpool} {materialName} ",
                            AlertResources.Yes,
                            AlertResources.No);
            if (decision)
            {
                var response = await Http.PutAsync($"api/management/order/mark-spended/{NodeId}/{materialId}/1", null);
                if (response.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert(
                            AlertResources.Confirmation,
                            AlertResources.Ready,
                            AlertResources.Accept);
                    _ = GetMaterials();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                            AlertResources.Confirmation,
                            AlertResources.Error,
                            AlertResources.Accept);
                }
            }
        }
    }
}
