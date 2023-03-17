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
    public class GrupoService : IGrupoService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IteraLogService _logService;

        public GrupoService(IServiceScopeFactory serviceScopeFactory, IteraLogService logService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logService = logService;
        }

        public async Task<List<Grupo>> GetAllGruposAsync()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await _logService.CreateLogAsync(new Log { Message = "Starting GetAllGruposAsync" });
            var Grupos = await dbContext.Grupos.Include(g => g.Empresas).ThenInclude(c => c.Custos).ToListAsync();
            await _logService.CreateLogAsync(new Log { Message = $"Finished GetAllGruposAsync, found {Grupos.Count} Grupos" });
            return Grupos;
        }

        public async Task<List<Grupo>> GetGruposByDateAsync(DateTime date)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await _logService.CreateLogAsync(new Log { Message = $"Starting GetGruposByDateAsync with date={date}" });
            var Grupos = await dbContext.Grupos.Include(g => g.Empresas).ThenInclude(c => c.Custos)
                .Where(g => g.DateIngestion <= date).ToListAsync();
            await _logService.CreateLogAsync(new Log { Message = $"Finished GetGruposByDateAsync with date={date}, found {Grupos.Count} Grupos" });
            return Grupos;
        }

        public async Task<Grupo> GetGrupoByIdAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await _logService.CreateLogAsync(new Log { Message = $"Starting GetGrupoByIdAsync with id={id}" });
            var Grupo = await dbContext.Grupos.Include(g => g.Empresas).ThenInclude(c => c.Custos)
                .FirstOrDefaultAsync(g => g.Id == id);
            await _logService.CreateLogAsync(new Log { Message = $"Finished GetGrupoByIdAsync with id={id}" });
            return Grupo;
        }

        public async Task<Grupo> CreateGrupoAsync(Grupo Grupo)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await _logService.CreateLogAsync(new Log { Message = $"Starting CreateGrupoAsync with Grupo={Grupo}" });
            var existingGrupo = await dbContext.Grupos.FindAsync(Grupo.Id);
            if (existingGrupo != null)
            {
                throw new ArgumentException($"Grupo with ID {Grupo.Id} already exists.");
            }

            await dbContext.Grupos.AddAsync(Grupo);
            await dbContext.SaveChangesAsync();
            await _logService.CreateLogAsync(new Log { Message = $"Finished CreateGrupoAsync with Grupo={Grupo}" });
            return Grupo;
        }

        public async Task UpdateGrupoAsync(Grupo Grupo)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Grupos.Update(Grupo);

            await dbContext.SaveChangesAsync();
            await _logService.CreateLogAsync(new Log { Message = $"Finished UpdateGrupoAsync with Grupo={Grupo}" });
        }

        public async Task DeleteGrupoAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var Grupo = await dbContext.Grupos.FindAsync(id);
            dbContext.Grupos.Remove(Grupo);
            await dbContext.SaveChangesAsync();
            await _logService.CreateLogAsync(new Log { Message = $"Finished DeleteGrupoAsync with id={id}" });
        }


        public async Task<Dictionary<string, Dictionary<string, float>>> GetCustosByGrupoIdAsync(int GrupoId)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var Grupo = await dbContext.Grupos.Include(g => g.Empresas).ThenInclude(c => c.Custos)
                                               .FirstOrDefaultAsync(g => g.Id == GrupoId);

            if (Grupo == null)
            {
                throw new ArgumentException($"Grupo with ID {GrupoId} não encontrado(a).");
            }

            var CustosByTypeAndYear = new Dictionary<string, Dictionary<string, float>>();

            foreach (var Empresa in Grupo.Empresas)
            {
                foreach (var Custo in Empresa.Custos)
                {
                    if (!CustosByTypeAndYear.ContainsKey(Custo.IdType))
                    {
                        CustosByTypeAndYear.Add(Custo.IdType, new Dictionary<string, float>());
                    }

                    if (!CustosByTypeAndYear[Custo.IdType].ContainsKey(Custo.Ano))
                    {
                        CustosByTypeAndYear[Custo.IdType].Add(Custo.Ano, 0);
                    }

                    CustosByTypeAndYear[Custo.IdType][Custo.Ano] += Custo.Value;
                }
            }

            return CustosByTypeAndYear;
        }




    }
    public interface IGrupoService
    {
        Task<List<Grupo>> GetAllGruposAsync();
        Task<List<Grupo>> GetGruposByDateAsync(DateTime date);
        Task<Grupo> GetGrupoByIdAsync(int id);
        Task<Dictionary<string, Dictionary<string, float>>> GetCustosByGrupoIdAsync(int GrupoId);
        Task<Grupo> CreateGrupoAsync(Grupo Grupo);
        Task UpdateGrupoAsync(Grupo Grupo);
        Task DeleteGrupoAsync(int id);
    }

}
