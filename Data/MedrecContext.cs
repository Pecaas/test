using Medrec.Models.medrec;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Medrec.Data
{
    public partial class medrecContext : DbContext
    {
        public medrecContext()
        {
        }

        public medrecContext(DbContextOptions<medrecContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            this.OnModelBuilding(builder);

            var isFirebird = Database.ProviderName == "FirebirdSql.EntityFrameworkCore.Firebird";

            if (isFirebird)
            {
                foreach (var entity in builder.Model.GetEntityTypes())
                {
                    entity.SetTableName(entity.GetTableName().ToUpper());
                    entity.SetSchema(null);

                    foreach (var property in entity.GetProperties())
                    {
                        property.SetColumnName(property.GetColumnName().ToUpper());
                    }

                    foreach (var key in entity.GetKeys())
                    {
                        key.SetName(key.GetName().ToUpper());
                    }

                    foreach (var fk in entity.GetForeignKeys())
                    {
                        fk.SetConstraintName(fk.GetConstraintName()?.ToUpper());
                    }

                    foreach (var index in entity.GetIndexes())
                    {
                        index.SetDatabaseName(index.GetDatabaseName()?.ToUpper());
                    }
                }
            }

        }

        public DbSet<Medrec.Models.medrec.T01pat> T01pats { get; set; }

        public DbSet<Medrec.Models.medrec.T01patid> T01patids { get; set; }

        public DbSet<Medrec.Models.medrec.T03event> T03events { get; set; }

        public DbSet<Medrec.Models.medrec.T02descr> T02descrs { get; set; }

        public DbSet<Medrec.Models.medrec.T02measure> T02measures { get; set; }

        public DbSet<Medrec.Models.medrec.T03measure> T03measures { get; set; }

        public DbSet<Medrec.Models.medrec.T90tab1> T90tab1S { get; set; }

        public DbSet<Medrec.Models.medrec.Version> Versions { get; set; }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }
}