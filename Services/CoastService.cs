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
        private readonly LogService _logService;

        public CostService(IServiceScopeFactory serviceScopeFactory, LogService logService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logService = logService;
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

            // Registro de log
            await _logService.CreateLogAsync(new Log { Message = $"Cost {cost.Id} created" });

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

            var costToUpdate = await dbContext.Costs.FindAsync(id);

            if (costToUpdate == null)
            {
                throw new Exception("Cost not found");
            }

            // Verificar se os valores foram atualizados
            bool valuesUpdated = false;
            if (costToUpdate.Value != CostIn.Value)
            {
                costToUpdate.Value = CostIn.Value;
                valuesUpdated = true;
            }
            if (costToUpdate.IdType != CostIn.IdType)
            {
                costToUpdate.IdType = CostIn.IdType;
                valuesUpdated = true;
            }
            if (costToUpdate.Ano != CostIn.Ano)
            {
                costToUpdate.Ano = CostIn.Ano;
                valuesUpdated = true;
            }

            if (valuesUpdated)
            {
                costToUpdate.LastUpdate = DateTime.UtcNow;
                await dbContext.SaveChangesAsync();

                // Registro de log
                await _logService.CreateLogAsync(new Log { Message = $"Cost {id} updated" });
            }

            return costToUpdate;
        }

        public async Task RemoveCostAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var costToRemove = await dbContext.Costs.FindAsync(id);

            if (costToRemove == null)
            {
                throw new Exception("Cost not found");
            }

            dbContext.Costs.Remove(costToRemove);
            await dbContext.SaveChangesAsync();

            // Registro de log
            await _logService.CreateLogAsync(new Log { Message = $"Cost {id} removed" });
        }
    }

}
