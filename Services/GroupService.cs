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

        public GroupService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public async Task<List<Group>> GetGroupsByDateAsync(DateTime date)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var groups = await dbContext.Groups.Include(g => g.Companys).ThenInclude(c => c.Costs)
                .Where(g => g.DateIngestion <= date).ToListAsync();

            return groups;
        }


        public async Task<List<Group>> GetAllGroupsAsync()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await dbContext.Groups.Include(g => g.Companys).ThenInclude(c => c.Costs).ToListAsync();
        }

        public async Task<Group> GetGroupByIdAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await dbContext.Groups.Include(g => g.Companys).ThenInclude(c => c.Costs).FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Group> CreateGroupAsync(Group group)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var existingGroup = await dbContext.Groups.FindAsync(group.Id);
            if (existingGroup != null)
            {
                throw new ArgumentException($"Group with ID {group.Id} already exists.");
            }

            await dbContext.Groups.AddAsync(group);
            await dbContext.SaveChangesAsync();
            return group;
        }


        public async Task UpdateGroupAsync(Group group)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Groups.Update(group);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteGroupAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var group = await dbContext.Groups.FindAsync(id);
            dbContext.Groups.Remove(group);
            await dbContext.SaveChangesAsync();
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
