using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class PersonsDbContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Person>()
                .HasIndex(c => c.Email)
                .IsUnique();


            modelBuilder.Entity<Country>()
                .Property(e => e.CountryName)
                .IsRequired();


            modelBuilder.Entity<Country>()
                .Property(e => e.CountryName)
                .HasMaxLength(80)
                .HasColumnName("Country_Name");
        }
    }
}



