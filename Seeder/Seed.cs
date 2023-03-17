using IteraEmpresaGrupos.Data;
using IteraEmpresaGrupos.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IteraEmpresaGrupos.Seeder
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

            var Grupos = SeedGrupos(companies);
            context.Grupos.AddRange(Grupos);
            context.SaveChanges();

            var Custos = SeedCustos(companies, context);
            context.Custos.AddRange(Custos);
            context.SaveChanges();
        }

        private static List<Empresa> SeedCompanies()
        {
            var companies = new List<Empresa>
            {
                new Empresa
                {
                    Name = "Empresa A",
                    Status = "ATIVO",
                    DateIngestion = new DateTime(2022, 8, 25, 8, 52, 0),
                    LastUpdate = new DateTime(2022, 8, 25, 8, 52, 0)
                },
                new Empresa
                {
                    Name = "Empresa B",
                    Status = "ATIVO",
                    DateIngestion = new DateTime(2022, 8, 25, 8, 52, 0),
                    LastUpdate = new DateTime(2022, 8, 25, 8, 52, 0)
                },
                new Empresa
                {
                    Name = "Empresa C",
                    Status = "INATIVO",
                    DateIngestion = new DateTime(2022, 8, 25, 8, 52, 0),
                    LastUpdate = new DateTime(2022, 8, 25, 8, 52, 0)
                },
                new Empresa
                {
                    Name = "Empresa D",
                    Status = "ATIVO",
                    DateIngestion = new DateTime(2022, 8, 25, 8, 52, 0),
                    LastUpdate = new DateTime(2022, 8, 25, 8, 52, 0)
                }
            };

            return companies;
        }

        private static List<Grupo> SeedGrupos(List<Empresa> companies)
        {
            var Grupos = new List<Grupo>
            {
                new Grupo
                {
                    Name = "Grupo A",
                    Category = "Categoria X",
                    DateIngestion = new DateTime(2022, 8, 25, 8, 52, 0),
                    LastUpdate = new DateTime(2022, 8, 25, 8, 52, 0),
                    Empresas = companies.Take(2).ToList()
                },
                new Grupo
                {
                    Name = "Grupo B",
                    Category = "Categoria Y",
                    DateIngestion = new DateTime(2022, 8, 25, 8, 52, 0),
                    LastUpdate = new DateTime(2022, 8, 25, 8, 52, 0),
                    Empresas = companies.Skip(2).ToList()
                }
            };

            return Grupos;
        }

        private static List<Custo> SeedCustos(List<Empresa> companies, AppDbContext context)
        {
            var random = new Random();
            var Custos = new List<Custo>();

            foreach (var Empresa in companies)
            {
                for (int year = 2018; year <= 2020; year++)
                {
                    var id = 1;
                    for (int month = 1; month <= 12; month++)
                    {
                        for (int day = 1; day <= 28; day++)
                        {
                            var Custo = new Custo
                            {
                                Id = id++,
                                Value = (float)(random.Next(100, 1000) * 1.1),
                                LastUpdate = new DateTime(year, month, day),
                                EmpresaId = (int)Empresa.Id,
                                Ano = year.ToString(),
                                IdType = "tipo_" + id
                            };


                            var existingCusto = context.Custos.Local.FirstOrDefault(c => c.Id == Custo.Id);
                            if (existingCusto != null)
                            {

                                context.Entry(existingCusto).State = EntityState.Detached;
                            }


                            context.Custos.Attach(Custo);
                            context.Entry(Custo).State = EntityState.Added;
                        }
                    }
                }
            }


            return Custos;
        }
    }
}

