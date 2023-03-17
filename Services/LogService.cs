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
    public class LogService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public LogService(IServiceScopeFactory serviceScopeFactory)
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
                throw new Exception("Log not found");
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
                throw new Exception("Log not found");
            }

            dbContext.Logs.Remove(logToRemove);
            await dbContext.SaveChangesAsync();
        }
    }
}
