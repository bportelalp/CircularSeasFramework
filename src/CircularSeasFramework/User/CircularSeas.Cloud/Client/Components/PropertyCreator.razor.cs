using System.Net.Http;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace CircularSeas.Cloud.Client.Components
{
    public partial class PropertyCreator
    {
        [Inject] public HttpClient Http { get; set; }
        [Parameter] public Models.Property Property { get; set; }

        

        private bool _editing { get; set; } = false;
        private bool _isNew { get; set; } = false;
        private string _editingCSS => _editing ? "form-control" : "form-control-plaintext";
        private bool _disabled => !_editing;

        protected override void OnParametersSet()
        {
            if(Property == null)
            {
                _editing = true;
                _isNew = true;
                Property = new Models.Property();
            }
            base.OnParametersSet();
        }

        private async void ApplyChanges()
        {
            if (_isNew)
            {
                var content = new StringContent(JsonConvert.SerializeObject(Property));
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var response = await Http.PostAsync("api/management/property/new", content);
                var code = response.StatusCode;
            }
        }
    }
}
