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
        [Inject] public HttpClient Http { get; set; }
        [Inject] public NavigationManager nm { get; set; }

        private List<Material> _materials = new List<Material>();
        private string _filterMaterial = string.Empty;
        private bool _loading = true;
        private Guid _ViewingMaterial = Guid.Empty;
        private bool _isNew { get; set; } = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var response = await Http.GetAsync("api/management/materials");
                _materials = await response.Content.ReadFromJsonAsync<List<Material>>();
                _loading = false;
                StateHasChanged();
            }
        }

        private async Task Delete(Models.Material material)
        {
            var response = await Http.DeleteAsync($"api/management/material/delete/{material.Id}");
        }
    }
}