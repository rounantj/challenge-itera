using IteraEmpresaGrupos.Data;
using IteraEmpresaGrupos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IteraEmpresaGrupos.Services
{
    public class CustoService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IteraLogService _logService;

        public CustoService(IServiceScopeFactory serviceScopeFactory, IteraLogService logService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logService = logService;
        }

        public async Task<List<Custo>> GetCustosAsync()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await dbContext.Custos.ToListAsync();
        }

        public async Task<Custo> GetCustoByIdAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await dbContext.Custos.FirstOrDefaultAsync(c => c.Id == id);
        }


        public async Task<ActionResult<Custo>> CreateCustoAsync(Custo Custo)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Verificar se já existe um Custo com o mesmo EmpresaId e Date
            var existingCusto = await dbContext.Custos.FirstOrDefaultAsync(c => c.EmpresaId == Custo.EmpresaId);
            if (existingCusto != null)
            {
                throw new ArgumentException($"A Custo with ID {Custo.Id} already exists.");
            }

            Custo.LastUpdate = DateTime.UtcNow;
            dbContext.Custos.Add(Custo);
            await dbContext.SaveChangesAsync();

            // Registro de log
            await _logService.CreateLogAsync(new Log { Message = $"Custo {Custo.Id} created" });

            return Custo;
        }

        public async Task<Custo> GetCustoByTypeAndYearAsync(int EmpresaId, string idType, string year)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await dbContext.Custos.FirstOrDefaultAsync(c => c.EmpresaId == EmpresaId && c.IdType == idType && c.Ano == year);
        }



        public async Task<Custo> UpdateCustoAsync(int id, Custo CustoIn)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var CustoToUpdate = await dbContext.Custos.FindAsync(id);

            if (CustoToUpdate == null)
            {
                throw new Exception("Custo não encontrado(a)");
            }

            // Verificar se os valores foram atualizados
            bool valuesUpdated = false;
            if (CustoToUpdate.Value != CustoIn.Value)
            {
                CustoToUpdate.Value = CustoIn.Value;
                valuesUpdated = true;
            }
            if (CustoToUpdate.IdType != CustoIn.IdType)
            {
                CustoToUpdate.IdType = CustoIn.IdType;
                valuesUpdated = true;
            }
            if (CustoToUpdate.Ano != CustoIn.Ano)
            {
                CustoToUpdate.Ano = CustoIn.Ano;
                valuesUpdated = true;
            }

            if (valuesUpdated)
            {
                CustoToUpdate.LastUpdate = DateTime.UtcNow;
                await dbContext.SaveChangesAsync();

                // Registro de log
                await _logService.CreateLogAsync(new Log { Message = $"Custo {id} updated" });
            }

            return CustoToUpdate;
        }

        public async Task RemoveCustoAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var CustoToRemove = await dbContext.Custos.FindAsync(id);

            if (CustoToRemove == null)
            {
                throw new Exception("Custo não encontrado(a)");
            }

            dbContext.Custos.Remove(CustoToRemove);
            await dbContext.SaveChangesAsync();

            // Registro de log
            await _logService.CreateLogAsync(new Log { Message = $"Custo {id} removed" });
        }
    }

}
