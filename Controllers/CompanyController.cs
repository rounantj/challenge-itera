using IteraCompanyGroups.Models;
using IteraCompanyGroups.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IteraCompanyGroups.Controllers
{
    [ApiController]
    [Route("empresa")]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyService _companyService;

        public CompanyController(CompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<ActionResult<IQueryable<Company>>> Get() =>
            Ok(await _companyService.GetCompaniesAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> Get(int id)
        {
            var company = await _companyService.GetCompanyByIdAsync(id);

            return company is not null ? Ok(company) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Company>> Create(CompanyRequest companyRequest)
        {

            try
            {
                var company = new Company
                {
                    Name = companyRequest.Name,
                    Id = companyRequest.Id,
                    Status = companyRequest.Status,
                    DateIngestion = companyRequest.DateIngestion ?? DateTime.Now,
                    LastUpdate = companyRequest.LastUpdate ?? DateTime.Now
                };

                var newCompany = await _companyService.CreateCompanyAsync(company);

                return CreatedAtAction(nameof(Get), new { id = company.Id }, newCompany);


            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CompanyRequest companyRequest)
        {
            var company = await _companyService.GetCompanyByIdAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            company.Name = companyRequest.Name;
            company.Status = companyRequest.Status;
            company.DateIngestion = companyRequest.DateIngestion ?? DateTime.Now;
            company.LastUpdate = companyRequest.LastUpdate ?? DateTime.Now;

            var newCompany = await _companyService.UpdateCompanyAsync(id, company);


            return CreatedAtAction(nameof(Get), new { id = company.Id }, company = newCompany);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var company = await _companyService.GetCompanyByIdAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            await _companyService.RemoveCompanyAsync(id);

            return NoContent();
        }

    }
}
