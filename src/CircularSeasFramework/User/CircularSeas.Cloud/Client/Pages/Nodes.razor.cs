using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Newtonsoft.Json;


namespace CircularSeas.Cloud.Client.Pages
{
    public partial class Nodes
    {
        [Inject] public HttpClient Http { get; set; }


        private List<Models.Node> _nodes = new();
        private Guid _currentNode = default(Guid);
        private List<Models.Material> _materialsCurrent = new();

        private bool _loading = true;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var response = await Http.GetAsync("api/management/nodes/list");
                if (response.IsSuccessStatusCode)
                {
                    _nodes = await response.Content.ReadFromJsonAsync<List<Models.Node>>();
                    _currentNode = _nodes.FirstOrDefault().Id;

                    await GetNodeStock();
                }
                _loading = false;
                StateHasChanged();
            }
        }


        private async Task GetNodeStock()
        {
            var queryMaterials = QueryHelpers.AddQueryString("api/management/materials", new Dictionary<string, string>
                    {
                        {"includeProperties", "false" },
                        {"nodeStock",_currentNode.ToString()}
                    });

            var response = await Http.GetAsync(queryMaterials);
            if (response.IsSuccessStatusCode)
            {
                _materialsCurrent = await response.Content.ReadFromJsonAsync<List<Models.Material>>();
            }
        }


        private async Task NavigateNode(Models.Node node)
        {
            _currentNode = node.Id;
            _loading = true;
            StateHasChanged();

            await GetNodeStock();

            _loading = false;
            StateHasChanged();
            
        }

        private string GetNodeCSS(Guid nodeId)
        {
            if (nodeId == _currentNode) return "tree-item__active";
            else return string.Empty;
        }
    }

}
