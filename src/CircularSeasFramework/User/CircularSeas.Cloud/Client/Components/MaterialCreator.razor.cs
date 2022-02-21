using System;
using System.Text.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CircularSeas.Cloud.Client.Components
{
    public partial class MaterialCreator
    {
        [Inject] public HttpClient Http { get; set; } 

        [Parameter] public Models.Material Material { get; set; } 


        private bool _editing { get; set; } = false;
        private string _editingCSS => _editing ? "form-control" : "form-control-plaintext";
        private bool _disabled => !_editing;
        private bool _loading = true;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Material = await Http.GetFromJsonAsync<Models.Material>("api/management/material/schema");
                _editing = true;
                _loading = false;
                StateHasChanged();
            }
        }

        private async Task SaveMaterial()
        {
            var content = new StringContent(JsonConvert.SerializeObject(Material));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await Http.PostAsync("api/management/material/new",content);

        }
    }
}
