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
    [Route("Grupo/[controller]")]
    public class CustoController : ControllerBase
    {
        private readonly EmpresaService _EmpresaService;
        private readonly CustoService _CustoService;

        public CustoController(EmpresaService EmpresaService, CustoService CustoService)
        {
            _EmpresaService = EmpresaService;
            _CustoService = CustoService;
        }

        [HttpGet]
        public async Task<ActionResult<IQueryable<Custo>>> Get() =>
            Ok(await _CustoService.GetCustosAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Custo>> Get(int id)
        {
            var Custo = await _CustoService.GetCustoByIdAsync(id);

            return Custo is not null ? Ok(Custo) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Custo>> Create(CustoRequest CustoRequest)
        {

            try
            {
                var Empresa = await _EmpresaService.GetEmpresaByIdAsync(CustoRequest.EmpresaId);
                if (Empresa == null)
                {
                    return NotFound("Empresa não encontrado(a)");
                }

                var Custo = new Custo
                {
                    Id = CustoRequest.Id,
                    IdType = CustoRequest.IdType,
                    Ano = CustoRequest.Ano,
                    Value = (float)CustoRequest.Value,
                    EmpresaId = CustoRequest.EmpresaId
                };

                var newCusto = await _CustoService.CreateCustoAsync(Custo);

                return CreatedAtAction(nameof(Get), new { id = Custo.Id }, newCusto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCusto(int id, CustoRequest CustoRequest)
        {
            try
            {
                var Empresa = await _EmpresaService.GetEmpresaByIdAsync(id);
                if (Empresa == null)
                {
                    return BadRequest("Empresa não encontrado(a)");
                }

                var existingCusto = await _CustoService.GetCustoByTypeAndYearAsync(id, CustoRequest.IdType, CustoRequest.Ano);
                if (existingCusto != null)
                {
                    existingCusto.Value = CustoRequest.Value;
                    await _CustoService.UpdateCustoAsync(existingCusto.Id, existingCusto);
                }
                else
                {
                    var newCusto = new Custo
                    {
                        IdType = CustoRequest.IdType,
                        Ano = CustoRequest.Ano,
                        Value = CustoRequest.Value,
                        EmpresaId = id
                    };
                    await _CustoService.CreateCustoAsync(newCusto);
                }

                return Ok(); // resposta 200
            }
            catch (Exception ex)
            {
                return StatusCode(500); // resposta 500
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var Custo = await _CustoService.GetCustoByIdAsync(id);

            if (Custo == null)
            {
                return NotFound();
            }

            await _CustoService.RemoveCustoAsync(id);

            return NoContent();
        }
    }
}
