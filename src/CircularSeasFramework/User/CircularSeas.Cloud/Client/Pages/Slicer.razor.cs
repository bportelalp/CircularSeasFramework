using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CircularSeas.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace CircularSeas.Cloud.Client.Pages
{
    public partial class Slicer
    {
        [Inject] public HttpClient Http { get; set; }
        [Inject] public IJSRuntime js { get; set; }

        private List<Material> _materials;
        private List<CircularSeas.Models.Slicer.Filament> _filaments;
        private SlicerForm slicerForm = new SlicerForm();

        private bool _loading = true;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender)
                {
                    var response = await Http.GetAsync($"api/process/available-materials/CS_Ultimaker2plus/{default(Guid)}");
                    var responseissue = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        var dto = await response.Content.ReadFromJsonAsync<Models.DTO.PrintDTO>();
                        _materials = dto.Materials;
                        _filaments = dto.Filaments;
                        _loading = false;
                        StateHasChanged();
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public void LoadFile(ChangeEventArgs e)
        {

        }

        public async Task Submit()
        {
            if(slicerForm.Print == Guid.Empty || slicerForm.Material == Guid.Empty)
            {
                await js.InvokeVoidAsync("alert", "Some of the fields are null");
                return;
            }
            try
            {
                
                using (var content = new MultipartFormDataContent())
                {
                    Stream result = slicerForm.STL.OpenReadStream(long.MaxValue);
                    var stream = new StreamContent(result);
                    stream.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/sla");
                    content.Add(stream, slicerForm.STL.Name, slicerForm.STL.Name);
                    var query = QueryHelpers.AddQueryString("api/process/slice",
                        new Dictionary<string, string>()
                        {
                            {"printer",slicerForm.Printer },
                            {"filament", _filaments.Find(f => f.MaterialFK == slicerForm.Material).Name },
                            {"print",_filaments.SelectMany(f => f.CompatiblePrints).Where(f => f.Id == slicerForm.Print).FirstOrDefault().Name },
                            {"support", slicerForm.Support == "Yes"? true.ToString():false.ToString() }
                        });
                    var response = await Http.PostAsync(query, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var stringReturn = await response.Content.ReadAsStringAsync();
                        await js.InvokeVoidAsync("DownloadPlainText",slicerForm.STL.Name.Replace(".stl",".gcode"), stringReturn);
                    }
                    else
                    {
                        await js.InvokeVoidAsync("alert", $"An error has been thrown: {await response.Content.ReadAsStringAsync()}");
                    }

                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
            finally
            {

            }

        }
    }





    public class SlicerForm
    {
        [Required]
        public IBrowserFile STL { get; set; }
        [Required]
        public string Printer { get; set; } = "CS_Ultimaker2plus";
        [Required]
        public Guid Material { get; set; }
        [Required]
        public Guid Print { get; set; }
        [Required]
        public string Support { get; set; }
    }
}
