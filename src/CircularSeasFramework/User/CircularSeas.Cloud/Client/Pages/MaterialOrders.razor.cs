using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace CircularSeas.Cloud.Client.Pages
{
    public partial class MaterialOrders
    {
        [Inject] public HttpClient Http { get; set; }
        [Inject] IJSRuntime js { get; set; }



        private bool _loading { get; set; } = true;
        private List<Models.Order> _orders { get; set; } = new List<Models.Order>();
        private int _currentSection = 1;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var response = await Http.GetAsync(QueryHelpers.AddQueryString("api/management/order/list", "status", _currentSection.ToString()));
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _orders = await response.Content.ReadFromJsonAsync<List<Models.Order>>();
                }
                _loading = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task ChangeSection(int number)
        {
            if (_currentSection != number)
            {
                _loading = true;
                StateHasChanged();
                _currentSection = number;

                var query = QueryHelpers.AddQueryString("api/management/order/list", "status", _currentSection.ToString());

                var response = await Http.GetAsync(query);
                if (response.IsSuccessStatusCode)
                {
                    _orders = await response.Content.ReadFromJsonAsync<List<Models.Order>>();
                }
                _loading = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task MarkDelivered(ChangeEventArgs e, Models.Order order, int section = 1)
        {
            if (section == 1)
            {
                order.ShippingDate = DateTime.Now;
                order.Delivered = true;
            }
            else
            {
                order.ShippingDate = null;
                order.Delivered = false;
            }
            var content = new StringContent(JsonConvert.SerializeObject(order));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await Http.PutAsync("api/management/order/update", content);
            if (response.IsSuccessStatusCode)
            {
                //Ok
            }
        }

        private List<Models.Order> SortedOrders()
        {
            if (_currentSection == 1)
                return _orders.OrderByDescending(o => o.CreationDate).ToList();
            else if (_currentSection == 2)
                return _orders.OrderByDescending(o => o.ShippingDate).ToList();
            else
                return _orders.OrderByDescending(o => o.FinishedDate).ToList();
        }

        private string GetDateSection()
        {
            switch (_currentSection)
            {
                case 1: return $"Creation date";
                case 2: return $"Shipping date";
                case 3: return $"Finished date";
                default: return string.Empty;
            }
        }
        private string GetDateSection(Models.Order order)
        {
            switch (_currentSection)
            {
                case 1: return $"{order.CreationDate.ToString("dd/MM/yy")} at {order.CreationDate.ToString("hh:mm tt")}";
                case 2: return $"{order.ShippingDate?.ToString("dd/MM/yy")} at {order.ShippingDate?.ToString("hh:mm tt")}";
                case 3: return $"{order.FinishedDate?.ToString("dd/MM/yy")} at {order.FinishedDate?.ToString("hh:mm tt")}";
                default: return string.Empty;
            }
        }
    }
}
