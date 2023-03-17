using IteraCompanyGroups.Data;
using IteraCompanyGroups.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IteraCompanyGroups.Services
{
    public class CostService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CostService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<List<Cost>> GetCostsAsync()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await dbContext.Costs.ToListAsync();
        }

        public async Task<Cost> GetCostByIdAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await dbContext.Costs.FirstOrDefaultAsync(c => c.Id == id);
        }


        public async Task<ActionResult<Cost>> CreateCostAsync(Cost cost)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Verificar se já existe um cost com o mesmo CompanyId e Date
            var existingCost = await dbContext.Costs.FirstOrDefaultAsync(c => c.CompanyId == cost.CompanyId);
            if (existingCost != null)
            {
                throw new ArgumentException($"A cost with ID {cost.Id} already exists.");
            }

            cost.LastUpdate = DateTime.UtcNow;
            dbContext.Costs.Add(cost);
            await dbContext.SaveChangesAsync();
            return cost;
        }

        public async Task<Cost> GetCostByTypeAndYearAsync(int companyId, string idType, string year)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await dbContext.Costs.FirstOrDefaultAsync(c => c.CompanyId == companyId && c.IdType == idType && c.Ano == year);
        }



        public async Task<Cost> UpdateCostAsync(int id, Cost CostIn)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var CostToUpdate = await dbContext.Costs.FindAsync(id);

            if (CostToUpdate == null)
            {
                throw new Exception("Cost not found");
            }

            CostToUpdate.Ano = CostIn.Ano;
            CostToUpdate.IdType = CostIn.IdType;
            CostToUpdate.Value = CostIn.Value;
            CostToUpdate.LastUpdate = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            return CostToUpdate;
        }

        public async Task RemoveCostAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var CostToRemove = await dbContext.Costs.FindAsync(id);

            if (CostToRemove == null)
            {
                throw new Exception("Cost not found");
            }

            dbContext.Costs.Remove(CostToRemove);
            await dbContext.SaveChangesAsync();
        }
    }
}
