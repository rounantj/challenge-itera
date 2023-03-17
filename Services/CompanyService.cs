using IteraEmpresaGrupos.Data;
using IteraEmpresaGrupos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IteraEmpresaGrupos.Services
{
    public class EmpresaService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IteraLogService _logService;

        public EmpresaService(IServiceScopeFactory serviceScopeFactory, IteraLogService logService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logService = logService;
        }

        public async Task<List<Empresa>> GetCompaniesAsync()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await dbContext.Companies.Include(c => c.Custos).ToListAsync();
        }


        public async Task<Empresa> GetEmpresaByIdAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await dbContext.Companies.Include(c => c.Custos).FirstOrDefaultAsync(Empresa => Empresa.Id == id);
        }


        public async Task<Empresa> CreateEmpresaAsync(Empresa Empresa)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // verifica se já existe uma empresa com o mesmo ID
            var existingEmpresa = await dbContext.Companies.FindAsync(Empresa.Id);
            if (existingEmpresa != null)
            {
                throw new ArgumentException($"A Empresa with ID {Empresa.Id} already exists.");
            }

            if (Empresa.Status != "ATIVO" && Empresa.Status != "INATIVO")
            {
                throw new ArgumentException($"A Empresa status can be 'INATIVO' or 'ATIVO'.");
            }

            Empresa.DateIngestion = DateTime.UtcNow;
            Empresa.LastUpdate = DateTime.UtcNow;
            dbContext.Companies.Add(Empresa);
            await dbContext.SaveChangesAsync();

            // Registro de log
            await _logService.CreateLogAsync(new Log { Message = $"Empresa {Empresa.Id} created" });

            return Empresa;
        }

        public async Task<Empresa> UpdateEmpresaAsync(int id, Empresa EmpresaIn)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var EmpresaToUpdate = await dbContext.Companies.FindAsync(id);

            if (EmpresaToUpdate == null)
            {
                throw new Exception("Empresa não encontrado(a)");
            }

            EmpresaToUpdate.Name = EmpresaIn.Name;
            EmpresaToUpdate.Status = EmpresaIn.Status;
            EmpresaToUpdate.LastUpdate = DateTime.UtcNow;
            EmpresaToUpdate.Custos = EmpresaIn.Custos;

            await dbContext.SaveChangesAsync();

            // Registro de log
            await _logService.CreateLogAsync(new Log { Message = $"Empresa {EmpresaToUpdate.Id} updated" });

            return EmpresaToUpdate;
        }

        public async Task RemoveEmpresaAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var empresaToInativar = await dbContext.Companies.FindAsync(id);

            if (empresaToInativar == null)
            {
                throw new Exception("Empresa não encontrado(a)");
            }

            empresaToInativar.Status = "INATIVO";
            await dbContext.SaveChangesAsync();

            // Registro de log
            await _logService.CreateLogAsync(new Log { Message = $"Empresa {id} inativada" });
        }
    }
}
