using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IteraEmpresaGrupos.Services;
using IteraEmpresaGrupos.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace IteraEmpresaGrupos.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[Controller]")]
    public class GrupoController : ControllerBase
    {
        private readonly IGrupoService _GrupoService;

        public GrupoController(IGrupoService GrupoService)
        {
            _GrupoService = GrupoService;
        }


        // Endpoints específicos do desafio

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Grupo>>> GetGruposByDate([FromQuery] string date)
        {
            DateTime parsedDate;
            if (!DateTime.TryParse(date, out parsedDate))
            {
                return BadRequest();
            }

            var Grupos = await _GrupoService.GetGruposByDateAsync(parsedDate);

            if (Grupos == null || Grupos.Count() == 0)
            {
                return NotFound();
            }

            return Ok(Grupos);
        }




        [HttpGet("{id}")]
        public async Task<ActionResult> GetCustosByGrupoIdAsync(int id)
        {
            var Grupo = await _GrupoService.GetGrupoByIdAsync(id);
            if (Grupo == null)
            {
                return NotFound("Grupo não encontrado(a)");
            }

            try
            {
                var Custos = await _GrupoService.GetCustosByGrupoIdAsync(id);
                return Ok(Custos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Grupo>> CreateGrupo([FromBody] GrupoRequest GrupoRequest)
        {

            var Grupo = new Grupo
            {
                Name = GrupoRequest.Name,
                Id = GrupoRequest.Id,
                Category = GrupoRequest.Category,
                DateIngestion = GrupoRequest.DateIngestion,
                LastUpdate = GrupoRequest.LastUpdate,
            };

            try
            {
                var createdGrupo = await _GrupoService.CreateGrupoAsync(Grupo);
                return createdGrupo;
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGrupo(int id, [FromBody] GrupoRequest GrupoRequest)
        {
            if (id != GrupoRequest.Id)
            {
                return BadRequest();
            }

            var existingGrupo = await _GrupoService.GetGrupoByIdAsync(id);
            if (existingGrupo == null)
            {
                return NotFound();
            }


            var Grupo = new Grupo
            {
                Id = GrupoRequest.Id,
                Name = GrupoRequest.Name,
                Category = GrupoRequest.Category,
                DateIngestion = GrupoRequest.DateIngestion,
                LastUpdate = GrupoRequest.LastUpdate,
            };

            await _GrupoService.UpdateGrupoAsync(Grupo);

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrupo(int id)
        {
            var existingGrupo = await _GrupoService.GetGrupoByIdAsync(id);
            if (existingGrupo == null)
            {
                return NotFound();
            }

            await _GrupoService.DeleteGrupoAsync(id);

            return NoContent();
        }
    }
}
