using System;
using System.Text.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CircularSeas.Cloud.Client.Components
{
    public partial class MaterialEditor
    {
        [Inject] public HttpClient Http { get; set; }

        [Parameter] public Models.Material Material { get; set; }
        [Parameter] public EventCallback<Models.Material> MaterialChange { get; set; }


        private bool _editing { get; set; } = false;
        private string _editingCSS => _editing ? "form-control" : "form-control-plaintext";
        private bool _disabled => !_editing;
        private bool _loading = true;
        private bool _isNew = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (Material == null)
                {
                    //The case when new material will be created
                    _isNew = true;
                    Material = await Http.GetFromJsonAsync<Models.Material>("api/management/material/schema");
                    _editing = true;
                    _loading = false;
                }
                else
                {
                    _isNew = false;
                    _editing = true;
                    _loading = false;
                }
                StateHasChanged();
            }
        }

        private async Task SaveMaterial()
        {
            await MaterialChange.InvokeAsync(Material);


        }
    }
}
