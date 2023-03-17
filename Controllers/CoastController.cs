using IteraCompanyGroups.Models;
using IteraCompanyGroups.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IteraCompanyGroups.Controllers
{
    [ApiController]
    [Route("grupo/custos")]
    public class CostController : ControllerBase
    {
        private readonly CompanyService _companyService;
        private readonly CostService _CostService;

        public CostController(CompanyService companyService, CostService CostService)
        {
            _companyService = companyService;
            _CostService = CostService;
        }

        [HttpGet]
        public async Task<ActionResult<IQueryable<Cost>>> Get() =>
            Ok(await _CostService.GetCostsAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Cost>> Get(int id)
        {
            var Cost = await _CostService.GetCostByIdAsync(id);

            return Cost is not null ? Ok(Cost) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Cost>> Create(CostRequest CostRequest)
        {

            try
            {
                var company = await _companyService.GetCompanyByIdAsync(CostRequest.CompanyId);
                if (company == null)
                {
                    return NotFound("Company not found");
                }

                var Cost = new Cost
                {
                    Id = CostRequest.Id,
                    IdType = CostRequest.IdType,
                    Ano = CostRequest.Ano,
                    Value = (float)CostRequest.Value,
                    CompanyId = CostRequest.CompanyId
                };

                var newCost = await _CostService.CreateCostAsync(Cost);

                return CreatedAtAction(nameof(Get), new { id = Cost.Id }, newCost);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCost(int id, CostRequest costRequest)
        {
            try
            {
                var company = await _companyService.GetCompanyByIdAsync(id);
                if (company == null)
                {
                    return BadRequest("Company not found");
                }

                var existingCost = await _CostService.GetCostByTypeAndYearAsync(id, costRequest.IdType, costRequest.Ano);
                if (existingCost != null)
                {
                    existingCost.Value = costRequest.Value;
                    await _CostService.UpdateCostAsync(existingCost.Id, existingCost);
                }
                else
                {
                    var newCost = new Cost
                    {
                        IdType = costRequest.IdType,
                        Ano = costRequest.Ano,
                        Value = costRequest.Value,
                        CompanyId = id
                    };
                    await _CostService.CreateCostAsync(newCost);
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
            var Cost = await _CostService.GetCostByIdAsync(id);

            if (Cost == null)
            {
                return NotFound();
            }

            await _CostService.RemoveCostAsync(id);

            return NoContent();
        }
    }
}
