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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var response = await Http.GetAsync(QueryHelpers.AddQueryString("api/management/order/list", "pending", "true"));
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    _orders = await response.Content.ReadFromJsonAsync<List<Models.Order>>();
                }
                _loading = false;
                await InvokeAsync(StateHasChanged);
            }
        }

    }
}
