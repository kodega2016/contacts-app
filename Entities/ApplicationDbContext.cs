using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Country> Countries { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // setting up the database table name
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            // seeding the database table with some data from JSON data
            // lets get the data from the JSON files first
            var dataDir = Path.Combine(AppContext.BaseDirectory, "Data");
            var countriesJson = File.ReadAllText(Path.Combine(dataDir, "countries.json"));
            var personsJson = File.ReadAllText(Path.Combine(dataDir, "persons.json"));

            // serialize the JSON data into model class
            var countries = JsonSerializer.Deserialize<List<Country>>(countriesJson);
            var persons = JsonSerializer.Deserialize<List<Person>>(personsJson);

            // seeding the data to the database
            modelBuilder.Entity<Country>().HasData(countries ?? []);
            modelBuilder.Entity<Person>().HasData(persons ?? []);

            // fluent api for database field properties
            modelBuilder.Entity<Person>().Property(temp => temp.TFN)
            .HasMaxLength(200).
            HasDefaultValue("ABC");
            // IsRequired(true);

            // setting up the unique constraint using fluent api
            modelBuilder.Entity<Person>().HasIndex("Email").IsUnique();

            // relationship
            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasOne<Country>(c => c.Country)
                .WithMany(p => p.Persons);
            });

        }
    }
}


