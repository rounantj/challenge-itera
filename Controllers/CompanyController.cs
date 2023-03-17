using IteraEmpresaGrupos.Models;
using IteraEmpresaGrupos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IteraEmpresaGrupos.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class EmpresaController : ControllerBase
    {
        private readonly EmpresaService _EmpresaService;

        public EmpresaController(EmpresaService EmpresaService)
        {
            _EmpresaService = EmpresaService;
        }

        [HttpGet]
        public async Task<ActionResult<IQueryable<Empresa>>> Get() =>
            Ok(await _EmpresaService.GetCompaniesAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Empresa>> Get(int id)
        {
            var Empresa = await _EmpresaService.GetEmpresaByIdAsync(id);

            return Empresa is not null ? Ok(Empresa) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Empresa>> Create(EmpresaRequest EmpresaRequest)
        {

            try
            {
                var Empresa = new Empresa
                {
                    Name = EmpresaRequest.Name,
                    Id = EmpresaRequest.Id,
                    Status = EmpresaRequest.Status,
                    DateIngestion = EmpresaRequest.DateIngestion ?? DateTime.Now,
                    LastUpdate = EmpresaRequest.LastUpdate ?? DateTime.Now
                };

                var newEmpresa = await _EmpresaService.CreateEmpresaAsync(Empresa);

                return CreatedAtAction(nameof(Get), new { id = Empresa.Id }, newEmpresa);


            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, EmpresaRequest EmpresaRequest)
        {
            var Empresa = await _EmpresaService.GetEmpresaByIdAsync(id);

            if (Empresa == null)
            {
                return NotFound();
            }

            Empresa.Name = EmpresaRequest.Name;
            Empresa.Status = EmpresaRequest.Status;
            Empresa.DateIngestion = EmpresaRequest.DateIngestion ?? DateTime.Now;
            Empresa.LastUpdate = EmpresaRequest.LastUpdate ?? DateTime.Now;

            var newEmpresa = await _EmpresaService.UpdateEmpresaAsync(id, Empresa);


            return CreatedAtAction(nameof(Get), new { id = Empresa.Id }, Empresa = newEmpresa);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var Empresa = await _EmpresaService.GetEmpresaByIdAsync(id);

            if (Empresa == null)
            {
                return NotFound();
            }

            await _EmpresaService.RemoveEmpresaAsync(id);

            return NoContent();
        }

    }
}
