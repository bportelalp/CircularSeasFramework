using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CircularSeas.Cloud.Client.Tools;
using CircularSeas.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace CircularSeas.Cloud.Client.Pages
{
    public partial class MaterialManager
    {
        [Inject] public HttpClient Http { get; set; }
        [Inject] public NavigationManager nm { get; set; }
        [Inject] public IJSRuntime js { get; set; }

        private List<Material> _materials = new List<Material>();
        private string _filterMaterial = string.Empty;
        private bool _loading = true;
        private Guid _ViewingMaterial = Guid.Empty;
        private Models.Material _editingMaterial = null;
        private StateEdition _state { get; set; } = StateEdition.None;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await GetMaterials();
                _loading = false;
                StateHasChanged();
            }
        }

        private async Task GetMaterials()
        {
            var response = await Http.GetAsync("api/management/materials");
            _materials = await response.Content.ReadFromJsonAsync<List<Material>>();
        }

        private async Task LaunchEdition(Models.Material material)
        {
            if (material == null)
            {
                _editingMaterial = await Http.GetFromJsonAsync<Models.Material>("api/management/material/schema");
                _state = StateEdition.Creating;
            }
            else
            {
                _editingMaterial = material;
                _state = StateEdition.Editing;
            }
            await InvokeAsync(StateHasChanged);
        }


        private async Task FinishEdition(Models.Material material)
        {
            if (material == null)
            {
                _state = StateEdition.None;
                await InvokeAsync(StateHasChanged);
                return;
            }
            var content = new StringContent(JsonConvert.SerializeObject(material));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = new HttpResponseMessage();
            if (_state == StateEdition.Creating)
            {
                response = await Http.PostAsync("api/management/material/new", content);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var created = await response.Content.ReadFromJsonAsync<Models.Material>();
                    await GetMaterials();
                    _state = StateEdition.None;
                    await InvokeAsync(StateHasChanged);
                }
                else
                {
                    await js.InvokeVoidAsync("alert", $"Ha ocurrido un error: {await response.Content.ReadAsStringAsync()}");
                }
            }
            else if (_state == StateEdition.Editing)
            {
                response = await Http.PutAsync("api/management/material/update-material", content);
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    _state = StateEdition.None;
                    await InvokeAsync(StateHasChanged);
                }
                else
                {
                    await js.InvokeVoidAsync("alert", $"Ha ocurrido un error: {await response.Content.ReadAsStringAsync()}");
                }
            }

        }

        private async Task DeprecateMaterial(Models.Material material)
        {
            var decision = await js.InvokeAsync<bool>("confirm", "Do you really want to mark as deprecated this material? This action cannot be undone");
            if (decision)
            {
                material.Deprecated = true;
                var content = new StringContent(JsonConvert.SerializeObject(material));
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var response = await Http.PutAsync("api/management/material/update-material", content);
                if (!response.IsSuccessStatusCode)
                {
                    await js.InvokeVoidAsync("alert", "Can't mar this material as deprecated");
                }
            }
        }
        private async Task DeleteMaterial(Models.Material material)
        {
            var decision = await js.InvokeAsync<bool>("confirm", "Do you really want to delete this material? All information will be deleted");
            if (decision)
            {
                var response = await Http.DeleteAsync($"api/management/material/delete/{material.Id}");
                if (response.StatusCode == HttpStatusCode.NoContent) _materials.Remove(material);
            }
        }

        private string PrintListFilaments(Models.Material material)
        {
            string list = string.Empty;
            material?.Filaments.ForEach(f => list += $"{f.Name}, ");
            return list;
        }
    }
}