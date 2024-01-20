using CityInfo.API.Entities;
using CityInfo.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace CityInfo.API.DbContexts;

public class CityInfoContext : DbContext
{
    public DbSet<City> Cities { get; set; } = null!;

    public DbSet<PointOfInterest> PointOfInterests { get; set; } = null!;

    public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>()
            .HasData(
                new City("Stoke-on-Trent")
                {
                    Id = 1,
                    Description = "Residing town."
                },
                new City("Chakswari")
                {
                    Id = 2,
                    Description = "Village home."
                },
                new City("Islamabad")
                {
                    Id = 3,
                    Description = "City home.",
                });

        modelBuilder.Entity<PointOfInterest>()
            .HasData(
            new PointOfInterest("The Potteries Museum & Art Gallery") 
            {
                Id = 1,
                CityId = 1,
                Description = "Displays locally made ceramics, decorative arts and a WWII Spitfire."
            },
            new PointOfInterest("Wedgwood Museum") {
                Id = 2,
                CityId = 1,
                Description = "£10 million Wedgwood Museum visitor centre."
            },
            new PointOfInterest("Mangla Dam")
            {
                Id = 3,
                CityId = 2,
                Description = "The sixth-largest dam in the world."
            },
            new PointOfInterest("Chakswari Grand Central Jamia Mosque")
            {
                Id = 4,
                CityId = 2,
                Description = "The main masjid in Chakswari."
            },
            new PointOfInterest("Faisal Mosque")
            {
                Id = 5,
                CityId = 3,
                Description = "The fifth-largest mosque in the world and the largest within South Asia, located on the foothills of Margalla Hills in Islamabad. It is named after the late King Faisal of Saudi Arabia."
            },
            new PointOfInterest("Pakistan Monument")
            {
                Id = 6,
                CityId = 3,
                Description = "A national monument and heritage museum located on the western Shakarparian Hills in Islamabad, Pakistan."
            });

        base.OnModelCreating(modelBuilder);
    }
}

