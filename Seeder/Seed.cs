using IteraCompanyGroups.Data;
using IteraCompanyGroups.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IteraCompanyGroups.Seeder
{
    public static class SeederClass
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var companies = SeedCompanies();
            context.Companies.AddRange(companies);
            context.SaveChanges();

            var groups = SeedGroups(companies);
            context.Groups.AddRange(groups);
            context.SaveChanges();

            var costs = SeedCosts(companies, context);
            context.Costs.AddRange(costs);
            context.SaveChanges();
        }

        private static List<Company> SeedCompanies()
        {
            var companies = new List<Company>
            {
                new Company
                {
                    Name = "Empresa A",
                    Status = "ATIVO",
                    DateIngestion = new DateTime(2022, 8, 25, 8, 52, 0),
                    LastUpdate = new DateTime(2022, 8, 25, 8, 52, 0)
                },
                new Company
                {
                    Name = "Empresa B",
                    Status = "ATIVO",
                    DateIngestion = new DateTime(2022, 8, 25, 8, 52, 0),
                    LastUpdate = new DateTime(2022, 8, 25, 8, 52, 0)
                },
                new Company
                {
                    Name = "Empresa C",
                    Status = "INATIVO",
                    DateIngestion = new DateTime(2022, 8, 25, 8, 52, 0),
                    LastUpdate = new DateTime(2022, 8, 25, 8, 52, 0)
                },
                new Company
                {
                    Name = "Empresa D",
                    Status = "ATIVO",
                    DateIngestion = new DateTime(2022, 8, 25, 8, 52, 0),
                    LastUpdate = new DateTime(2022, 8, 25, 8, 52, 0)
                }
            };

            return companies;
        }

        private static List<Group> SeedGroups(List<Company> companies)
        {
            var groups = new List<Group>
            {
                new Group
                {
                    Name = "Grupo A",
                    Category = "Categoria X",
                    DateIngestion = new DateTime(2022, 8, 25, 8, 52, 0),
                    LastUpdate = new DateTime(2022, 8, 25, 8, 52, 0),
                    Companys = companies.Take(2).ToList()
                },
                new Group
                {
                    Name = "Grupo B",
                    Category = "Categoria Y",
                    DateIngestion = new DateTime(2022, 8, 25, 8, 52, 0),
                    LastUpdate = new DateTime(2022, 8, 25, 8, 52, 0),
                    Companys = companies.Skip(2).ToList()
                }
            };

            return groups;
        }

        private static List<Cost> SeedCosts(List<Company> companies, AppDbContext context)
        {
            var random = new Random();
            var costs = new List<Cost>();

            foreach (var company in companies)
            {
                for (int year = 2018; year <= 2020; year++)
                {
                    var id = 1;
                    for (int month = 1; month <= 12; month++)
                    {
                        for (int day = 1; day <= 28; day++)
                        {
                            var cost = new Cost
                            {
                                Id = id++,
                                Value = (float)(random.Next(100, 1000) * 1.1),
                                LastUpdate = new DateTime(year, month, day),
                                CompanyId = (int)company.Id,
                                Ano = year.ToString(),
                                IdType = "tipo_" + id
                            };

                            // Check if a Cost object with the same key value is already being tracked
                            var existingCost = context.Costs.Local.FirstOrDefault(c => c.Id == cost.Id);
                            if (existingCost != null)
                            {
                                // Detach the existing Cost object so a new one with the same key value can be attached
                                context.Entry(existingCost).State = EntityState.Detached;
                            }

                            // Attach the new Cost object
                            context.Costs.Attach(cost);
                            context.Entry(cost).State = EntityState.Added;
                        }
                    }
                }
            }


            return costs;
        }
    }
}

