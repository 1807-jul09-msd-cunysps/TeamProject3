namespace WebAPI.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CreditHistoryDB : DbContext
    {
        public CreditHistoryDB()
            : base("name=CreditHistoryDB")
        {
        }

        public virtual DbSet<CreditHistory> CreditHistories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CreditHistory>()
                .Property(e => e.SSN)
                .IsFixedLength()
                .IsUnicode(false);
        }
    }
}
