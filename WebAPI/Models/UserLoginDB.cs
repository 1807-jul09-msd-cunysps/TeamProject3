namespace WebAPI.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class UserLoginDB : DbContext
    {
        public UserLoginDB()
            : base("name=UserLoginDB")
        {
        }

        public virtual DbSet<UserLogin> UserLogins { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserLogin>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<UserLogin>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<UserLogin>()
                .Property(e => e.GUID)
                .IsUnicode(false);
        }
    }
}
