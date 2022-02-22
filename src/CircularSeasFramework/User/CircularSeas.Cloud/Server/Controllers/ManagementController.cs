using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CircularSeas.DB;
using CircularSeas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CircularSeas.Cloud.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManagementController : Controller
    {
        private readonly DbService dbService;

        public ManagementController(DbService dbService)
        {
            this.dbService = dbService;
        }

        #region GETs
        [HttpGet("materials")]
        public async Task<IActionResult> GetMaterials()
        {
            try
            {
                var result = await dbService.GetMaterials(true, Guid.Empty, false);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }

        [HttpGet("material/detail/{materialId}")]
        public async Task<IActionResult> GetMaterialDetail([FromRoute] Guid materialId)
        {
            try
            {
                var result = await dbService.GetMaterialDetail(materialId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }

        }

        [HttpGet("properties")]
        public async Task<IActionResult> GetProperties()
        {
            try
            {
                var result = await dbService.GetProperties();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
                throw;
            }
        }

        [HttpGet("property/detail/{propertyId}")]
        public async Task<IActionResult> GetPropertyDetail([FromRoute] Guid propertyId)
        {
            try
            {
                var result = await dbService.GetPropertyDetail(propertyId);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
                throw;
            }
        }

        [HttpGet("material/schema")]
        public async Task<IActionResult> GetMaterialSchema()
        {
            try
            {
                var schema = await dbService.GetMaterialSchema();
                return Ok(schema);
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
                throw;
            }
        }

        [HttpGet("order/list")]
        public async Task<IActionResult> GetOrders([FromQuery] bool pending = true, [FromQuery] Guid nodeId = default(Guid))
        {
            try
            {
                var orders = await dbService.GetOrders(pending, nodeId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
                throw;
            }
        }
        #endregion


        #region POSTs
        [HttpPost("property/new")]
        public async Task<IActionResult> PostProperty([FromBody] Models.Property property)
        {
            try
            {
                var created = await dbService.CreateProperty(property);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
                throw;
            }
        }

        [HttpPost("material/new")]
        public async Task<IActionResult> PostMaterial([FromBody] Models.Material material)
        {
            try
            {
                var created = await dbService.CreateMaterial(material);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest();
                throw;
            }
        }
        #endregion

        #region PUTs
        [HttpPut("property/visibility/{propertyId}/{visible}")]
        public async Task<IActionResult> PutPropertyVisibility([FromRoute] Guid propertyId, [FromRoute] bool visible)
        {
            try
            {
                var incorrectMaterials = await dbService.CheckBadMaterialsVisible(propertyId);
                incorrectMaterials = null; //TODO
                if (incorrectMaterials == null)
                {
                    await dbService.UpdateVisibility(propertyId, visible);
                    return Ok("Bieeenn");
                }
                else
                {
                    return Conflict(incorrectMaterials);
                }
            }
            catch (Exception ex)
            {
                throw;
                return BadRequest(ex.Message);

            }
        }


        [HttpPut("material/update-properties")]
        public async Task<IActionResult> PutChangeMaterialProperties([FromBody] Models.Material material)
        {
            try
            {
                await dbService.UpdateMaterialEvaluations(material);
                return NoContent();
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
                throw;
            }

            return NoContent();
        }

        [HttpPut("material/update-material")]
        public async Task<IActionResult> PutUpdateMaterial([FromBody] Models.Material material)
        {
            try
            {
                await dbService.UpdateMaterial(material);
                return NoContent();
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
                throw;
            }
        }

        [HttpPut("property/update-property")]
        public async Task<IActionResult> PutUpdateProperty([FromBody] Models.Property property)
        {
            try
            {
                await dbService.UpdateProperty(property);
                return NoContent();
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
                throw;
            }
        }
        #endregion

        #region DELETE
        [HttpDelete("property/delete/{propertyId}")]
        public async Task<IActionResult> DeleteProperty([FromRoute] Guid propertyId)
        {
            try
            {
                await dbService.DeleteProperty(propertyId);
                return NoContent();
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
                throw;
            }
        }

        [HttpDelete("material/delete/{materialId}")]
        public async Task<IActionResult> DeleteMaterial([FromRoute] Guid materialId)
        {
            try
            {
                await dbService.DeleteMaterial(materialId);
                return NoContent();
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
                throw;
            }
        }
        #endregion
    }
}
