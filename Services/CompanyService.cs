using IteraCompanyGroups.Data;
using IteraCompanyGroups.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IteraCompanyGroups.Services
{
    public class CompanyService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly LogService _logService;

        public CompanyService(IServiceScopeFactory serviceScopeFactory, LogService logService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logService = logService;
        }

        public async Task<List<Company>> GetCompaniesAsync()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await dbContext.Companies.Include(c => c.Costs).ToListAsync();
        }


        public async Task<Company> GetCompanyByIdAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await dbContext.Companies.Include(c => c.Costs).FirstOrDefaultAsync(company => company.Id == id);
        }


        public async Task<Company> CreateCompanyAsync(Company company)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // verifica se já existe uma empresa com o mesmo ID
            var existingCompany = await dbContext.Companies.FindAsync(company.Id);
            if (existingCompany != null)
            {
                throw new ArgumentException($"A company with ID {company.Id} already exists.");
            }

            if (company.Status != "ATIVO" && company.Status != "INATIVO")
            {
                throw new ArgumentException($"A company status can be 'INATIVO' or 'ATIVO'.");
            }

            company.DateIngestion = DateTime.UtcNow;
            company.LastUpdate = DateTime.UtcNow;
            dbContext.Companies.Add(company);
            await dbContext.SaveChangesAsync();

            // Registro de log
            await _logService.CreateLogAsync(new Log { Message = $"Company {company.Id} created" });

            return company;
        }

        public async Task<Company> UpdateCompanyAsync(int id, Company companyIn)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var companyToUpdate = await dbContext.Companies.FindAsync(id);

            if (companyToUpdate == null)
            {
                throw new Exception("Company not found");
            }

            companyToUpdate.Name = companyIn.Name;
            companyToUpdate.Status = companyIn.Status;
            companyToUpdate.LastUpdate = DateTime.UtcNow;
            companyToUpdate.Costs = companyIn.Costs;

            await dbContext.SaveChangesAsync();

            // Registro de log
            await _logService.CreateLogAsync(new Log { Message = $"Company {companyToUpdate.Id} updated" });

            return companyToUpdate;
        }

        public async Task RemoveCompanyAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var companyToRemove = await dbContext.Companies.FindAsync(id);

            if (companyToRemove == null)
            {
                throw new Exception("Company not found");
            }

            dbContext.Companies.Remove(companyToRemove);
            await dbContext.SaveChangesAsync();

            // Registro de log
            await _logService.CreateLogAsync(new Log { Message = $"Company {companyToRemove.Id} removed" });
        }
    }
}
