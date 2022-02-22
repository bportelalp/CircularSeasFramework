using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace CircularSeas.Cloud.Client.Components
{
    public partial class PropertyEditor
    {
        [Inject] public HttpClient Http { get; set; }
        [Parameter] public Models.Property Property { get; set; }
        [Parameter] public EventCallback<Models.Property> PropertyChange { get; set;}

        

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

        private async Task ApplyChanges()
        {
            await PropertyChange.InvokeAsync(Property);
        }

    }
}
