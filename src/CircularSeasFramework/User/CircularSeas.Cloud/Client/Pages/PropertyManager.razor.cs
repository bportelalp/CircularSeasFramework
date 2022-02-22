using System;
using System.Collections.Generic;
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
    public partial class PropertyManager
    {
        [Inject] public HttpClient Http { get; set; }
        [Inject] public IJSRuntime js { get; set; }

        private List<Models.Property> _properties { get; set; }
        private bool _loading { get; set; } = true;
        private bool _newProperty { get; set; } = false;
        private string _filterProperty { get; set; } = string.Empty;
        private Guid _viewingProperty { get; set; } = Guid.Empty;

        private Models.Property _editingProperty { get; set; }
        private StateEdition _state;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var response = await Http.GetAsync("api/management/properties");
                _properties = await response.Content.ReadFromJsonAsync<List<Models.Property>>();
                _loading = false;
                StateHasChanged();
            }
        }


        private async Task ChangeVisibility(ChangeEventArgs e, Models.Property property)
        {
            if ((bool)e.Value)
            {
                var result = await js.InvokeAsync<bool>("confirm", "Changing the visibility means that users can use it for the decision support system. Make sure all materials are going to have this property covered before making it visible to avoid system instabilities. Are you sure you want to continue?");
                if (result)
                {
                    var response1 = await Http.PutAsync($"api/management/property/visibility/{property.Id}/true", null);
                    if (response1.StatusCode == HttpStatusCode.OK)
                    {
                        property.Visible = true;
                    }

                }
                else
                    property.Visible = false;
            }
            else
            {
                var result = await js.InvokeAsync<bool>("confirm", "This property will no longer be evaluable in the decision support for the user. However, the material values will remain unless the property is removed. Are you sure you want to continue?");
                if (result)
                {
                    var response2 = await Http.PutAsync($"api/management/property/visibility/{property.Id}/false", null);
                    if (response2.StatusCode == HttpStatusCode.OK)
                    {
                        property.Visible = false;
                    }

                }
                else
                    property.Visible = true;
            }
        }

        private void LaunchEdition(Models.Property property)
        {
            if (property == null)
            {
                _editingProperty = new Property();
                _state = StateEdition.Creating;
            }
            else
            {
                _editingProperty = property;
                _state = StateEdition.Editing;
            }
        }

        private async Task FinishEdition(Models.Property property)
        {
            if (property != null)
            {
                var content = new StringContent(JsonConvert.SerializeObject(property));
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var response = new HttpResponseMessage();
                if (_state == StateEdition.Creating)
                {
                    response = await Http.PostAsync("api/management/property/new", content);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var created = await response.Content.ReadFromJsonAsync<Models.Property>();
                        _properties.Add(created);
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
                    response = await Http.PutAsync("api/management/property/update-property", content);
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
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
            else
            {
                _state = StateEdition.None;
            }

        }

        private async Task Delete(Models.Property property)
        {
            var response = await Http.DeleteAsync($"api/management/property/delete/{property.Id}");
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
