using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CircularSeas.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CircularSeas.Cloud.Client.Pages
{
    public partial class PropertyManagement
    {
        [Inject] public HttpClient http { get; set; }
        [Inject] public IJSRuntime js { get; set; }

        private List<Models.Property> _properties { get; set; }
        private bool _loading { get; set; } = true;
        private bool _newProperty { get; set; } = false;
        private string _filterProperty { get; set; } = string.Empty;
        private Guid _viewingProperty { get; set; } = Guid.Empty;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var response = await http.GetAsync("api/management/properties");
                _properties = await response.Content.ReadFromJsonAsync<List<Models.Property>>();
                _loading = false;
                StateHasChanged();
            }
        }

        private async Task Delete(Models.Property property)
        {
            var response = await http.DeleteAsync($"api/management/property/delete/ee");
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                _properties.Remove(property);
            }
            else
            {
                var text = await response.Content.ReadAsStringAsync();
                await js.InvokeVoidAsync("alert", $"Ups! Parece que algo no ha ido bien: {text}");
            }

        }
    }
}
