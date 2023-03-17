using IteraEmpresaGrupos.Data;
using IteraEmpresaGrupos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IteraEmpresaGrupos.Services
{
    public class IteraLogService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public IteraLogService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<List<Log>> GetLogsAsync()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await dbContext.Logs.ToListAsync();
        }

        public async Task<Log> GetLogByIdAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await dbContext.Logs.FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Log> CreateLogAsync(Log log)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            log.Timestamp = DateTime.UtcNow;

            dbContext.Logs.Add(log);
            await dbContext.SaveChangesAsync();

            return log;
        }



        public async Task<Log> UpdateLogAsync(int id, Log logIn)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var logToUpdate = await dbContext.Logs.FindAsync(id);

            if (logToUpdate == null)
            {
                throw new Exception("Log não encontrado(a)");
            }

            logToUpdate.Message = logIn.Message;
            logToUpdate.Timestamp = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            return logToUpdate;
        }

        public async Task RemoveLogAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var logToRemove = await dbContext.Logs.FindAsync(id);

            if (logToRemove == null)
            {
                throw new Exception("Log não encontrado(a)");
            }

            dbContext.Logs.Remove(logToRemove);
            await dbContext.SaveChangesAsync();
        }
    }
}
