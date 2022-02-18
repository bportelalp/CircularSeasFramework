using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CircularSeas.Models;
using Microsoft.AspNetCore.Components;

namespace CircularSeas.Cloud.Client.Pages
{
    public partial class MaterialManagement
    {
        [Inject] HttpClient http { get; set; }

        private List<Material> _materials = new List<Material>();
        private string _filterMaterial = string.Empty;
        private bool _loading = true;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var response = await http.GetAsync("api/management/materials");
                _materials = await response.Content.ReadFromJsonAsync<List<Material>>();
                _loading = false;
                StateHasChanged();
            }
        }
    }
}
