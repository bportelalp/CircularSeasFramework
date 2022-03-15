using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CircularSeas.Cloud.Client.Tools;
using CircularSeas.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace CircularSeas.Cloud.Client.Pages
{
    public partial class SlicerConfig
    {
        [Inject] public HttpClient Http { get; set; }
        [Inject] public IJSRuntime js { get; set; }

        private bool _loading = false;
        private int _stage = 0;
        IBrowserFile _browserFile;

        private List<string> _filaments = new List<string>();
        private List<string> _prints = new List<string>();
        private List<string> _printers = new List<string>();
        private List<Models.Material> _materials = new List<Material>();
        private Dictionary<string, Guid> _matching = new Dictionary<string, Guid>();
        private Guid _draggingMaterial = Guid.Empty;




        private void LoadFile(InputFileChangeEventArgs e)
        {
            _browserFile = e.File;
            _stage = 0;
        }

        private async Task AnalizeFirstStep()
        {
            _loading = true;
            StateHasChanged();
            _filaments.Clear();
            try
            {
                if (_browserFile != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await _browserFile.OpenReadStream().CopyToAsync(ms);
                        using (StreamReader sr = new StreamReader(ms))
                        {
                            ms.Seek(0, SeekOrigin.Begin);
                            var lines = sr.ReadToEnd().Split(Environment.NewLine).ToList();

                            var filamentlines = lines.Where(l => l.StartsWith("[filament:")).ToList();
                            foreach (var line in filamentlines)
                            {
                                _filaments.Add(GetSettingBlockName(line));
                            }


                            var printslines = lines.Where(l => l.StartsWith("[print:")).ToList();
                            foreach (var line in printslines)
                            {
                                _prints.Add(GetSettingBlockName(line));
                            }
                            var printerlines = lines.Where(l => l.StartsWith("[printer:")).ToList();
                            foreach (var line in printerlines)
                            {
                                _printers.Add(GetSettingBlockName(line));
                            }
                            _stage++;
                        }
                    }

                    var query = QueryHelpers.AddQueryString("api/management/materials", new Dictionary<string, string>
                    {
                        {"includeProperties","false" },
                        {"forUsers","false" }
                    });
                    var response = await Http.GetAsync(query);
                    if (response.IsSuccessStatusCode)
                    {
                        _materials = await response.Content.ReadFromJsonAsync<List<Models.Material>>();
                    }
                }
                else
                {
                    await js.InvokeVoidAsync("alert", "Can't find a compatible file");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                _loading = false;
                StateHasChanged();
            }
        }

        private async Task SaveConfig()
        {
            _loading = true;
            StateHasChanged();
            try
            {
                using (var ms = new MemoryStream())
                {
                    await _browserFile.OpenReadStream().CopyToAsync(ms);
                    using (StreamReader sr = new StreamReader(ms))
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        var lines = sr.ReadToEnd().Split(Environment.NewLine).ToList();
                        List<string> lines2 = new List<string>();
                        lines.ForEach(line => lines2.Add(line.Trim('\r')));
                        var dto = new Models.DTO.ConfigDTO()
                        {
                            Matching = _matching,
                            Lines = lines2
                        };

                        var content = new StringContent(JsonConvert.SerializeObject(dto));
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        var response = await Http.PostAsync("api/management/settings/bundle-lines", content);
                        var message = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            await js.InvokeVoidAsync("alert", "Success");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _loading = false;
                StateHasChanged();
            }


        }
        private void DragStart(Guid materialId)
        {
            _draggingMaterial = materialId;
        }

        private void DragEnd(string filament)
        {
            if (_matching.ContainsKey(filament))
            {
                _matching.Remove(filament);
            }
            _matching.Add(filament, _draggingMaterial);
        }

        private string BorderColor(string field = "")
        {
            if (!string.IsNullOrEmpty(field))
                return _matching.ContainsKey(field) ? "border-color: green" : "border-color: gray";
            return "border-color: green";
        }

        private string GetSettingBlockName(string keyword)
        {
            var last = keyword.Split(":").Last();
            last = last.Remove(last.Length - 2);
            return last;
        }

    }
}
