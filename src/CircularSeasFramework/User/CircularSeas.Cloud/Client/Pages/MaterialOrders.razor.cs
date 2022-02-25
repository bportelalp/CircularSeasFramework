using System;
using System.Collections.Generic;
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

    }
}
