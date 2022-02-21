using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace CircularSeas.Cloud.Client.Components
{
    public partial class PropertiesEditor
    {
        [Inject] public HttpClient Http { get; set; }
        [Parameter] public Models.Material Material { get; set; }
        [Parameter] public EventCallback<Models.Material> OnMaterialEdited { get; set; }

        private bool _editing = false;
        private string _editingCSS => _editing ? "form-control" : "form-control-plaintext";
        private bool _disabled => !_editing;
        private Models.Material _unchangedMaterial;
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
        }

        private void ManageEdition()
        {
            if (!_editing)
            {
                _editing = true;
                _unchangedMaterial = JsonConvert.DeserializeObject<Models.Material>(JsonConvert.SerializeObject(Material));
            }
            else
            {
                _editing = false;
                Material = JsonConvert.DeserializeObject<Models.Material>(JsonConvert.SerializeObject(_unchangedMaterial));
            }
        } 


        private async Task ApplyChanges()
        {
            var content = new StringContent(JsonConvert.SerializeObject(Material));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await Http.PutAsync("api/management/material/update-properties", content);
            
            _editing = false;
        }
    }
}
