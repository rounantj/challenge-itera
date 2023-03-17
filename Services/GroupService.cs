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
    public class GroupService : IGroupService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly LogService _logService;

        public GroupService(IServiceScopeFactory serviceScopeFactory, LogService logService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logService = logService;
        }

        public async Task<List<Group>> GetAllGroupsAsync()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await _logService.CreateLogAsync(new Log { Message = "Starting GetAllGroupsAsync" });
            var groups = await dbContext.Groups.Include(g => g.Companys).ThenInclude(c => c.Costs).ToListAsync();
            await _logService.CreateLogAsync(new Log { Message = $"Finished GetAllGroupsAsync, found {groups.Count} groups" });
            return groups;
        }

        public async Task<List<Group>> GetGroupsByDateAsync(DateTime date)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await _logService.CreateLogAsync(new Log { Message = $"Starting GetGroupsByDateAsync with date={date}" });
            var groups = await dbContext.Groups.Include(g => g.Companys).ThenInclude(c => c.Costs)
                .Where(g => g.DateIngestion <= date).ToListAsync();
            await _logService.CreateLogAsync(new Log { Message = $"Finished GetGroupsByDateAsync with date={date}, found {groups.Count} groups" });
            return groups;
        }

        public async Task<Group> GetGroupByIdAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await _logService.CreateLogAsync(new Log { Message = $"Starting GetGroupByIdAsync with id={id}" });
            var group = await dbContext.Groups.Include(g => g.Companys).ThenInclude(c => c.Costs)
                .FirstOrDefaultAsync(g => g.Id == id);
            await _logService.CreateLogAsync(new Log { Message = $"Finished GetGroupByIdAsync with id={id}" });
            return group;
        }

        public async Task<Group> CreateGroupAsync(Group group)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await _logService.CreateLogAsync(new Log { Message = $"Starting CreateGroupAsync with group={group}" });
            var existingGroup = await dbContext.Groups.FindAsync(group.Id);
            if (existingGroup != null)
            {
                throw new ArgumentException($"Group with ID {group.Id} already exists.");
            }

            await dbContext.Groups.AddAsync(group);
            await dbContext.SaveChangesAsync();
            await _logService.CreateLogAsync(new Log { Message = $"Finished CreateGroupAsync with group={group}" });
            return group;
        }

        public async Task UpdateGroupAsync(Group group)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Groups.Update(group);

            await dbContext.SaveChangesAsync();
            await _logService.CreateLogAsync(new Log { Message = $"Finished UpdateGroupAsync with group={group}" });
        }

        public async Task DeleteGroupAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var group = await dbContext.Groups.FindAsync(id);
            dbContext.Groups.Remove(group);
            await dbContext.SaveChangesAsync();
            await _logService.CreateLogAsync(new Log { Message = $"Finished DeleteGroupAsync with id={id}" });
        }


        public async Task<Dictionary<string, Dictionary<string, float>>> GetCostsByGroupIdAsync(int groupId)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var group = await dbContext.Groups.Include(g => g.Companys).ThenInclude(c => c.Costs)
                                               .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
            {
                throw new ArgumentException($"Group with ID {groupId} not found.");
            }

            var costsByTypeAndYear = new Dictionary<string, Dictionary<string, float>>();

            foreach (var company in group.Companys)
            {
                foreach (var cost in company.Costs)
                {
                    if (!costsByTypeAndYear.ContainsKey(cost.IdType))
                    {
                        costsByTypeAndYear.Add(cost.IdType, new Dictionary<string, float>());
                    }

                    if (!costsByTypeAndYear[cost.IdType].ContainsKey(cost.Ano))
                    {
                        costsByTypeAndYear[cost.IdType].Add(cost.Ano, 0);
                    }

                    costsByTypeAndYear[cost.IdType][cost.Ano] += cost.Value;
                }
            }

            return costsByTypeAndYear;
        }




    }
    public interface IGroupService
    {
        Task<List<Group>> GetAllGroupsAsync();
        Task<List<Group>> GetGroupsByDateAsync(DateTime date);
        Task<Group> GetGroupByIdAsync(int id);
        Task<Dictionary<string, Dictionary<string, float>>> GetCostsByGroupIdAsync(int groupId);
        Task<Group> CreateGroupAsync(Group group);
        Task UpdateGroupAsync(Group group);
        Task DeleteGroupAsync(int id);
    }

}
