using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ef_core_cosmos_db_where_embedded_navigation_property_null
{
  class Program
  {
    static async Task Main(string[] args)
    {
      // Seed the database
      using (var appDbContext = new AppDbContext())
      {
        await appDbContext.Database.EnsureDeletedAsync();
        await appDbContext.Database.EnsureCreatedAsync();
        await appDbContext.Items.AddAsync(new Item
        {
          Name = "sole item",
          Metadata = new Metadata
          {
            CreatedBy = "me",
            UpdatedBy = "me"
          }
        });

        await appDbContext.SaveChangesAsync();
      }

      // Reproduce the problem knowing we have a single `Item` entity with a non-`null` `Metadata` navigation property
      using (var appDbContext = new AppDbContext())
      {
        var allItems = await appDbContext.Items.ToArrayAsync();
        var items = appDbContext.Items.Where(item => item.Metadata != null).ToArray();
        Console.WriteLine($"{nameof(allItems)} count: {allItems.Length}");

        // TODO: Distill more of the real application so that this reproduces by returning a zero
        Console.WriteLine($"{nameof(items)} count: {items.Length}");
      }
    }
  }

  class AppDbContext : DbContext
  {
    public DbSet<Item> Items { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      Console.WriteLine("Enter the CosmosDB Emulator primary key:");
      optionsBuilder.UseCosmos("https://localhost:8081", Console.ReadLine(), nameof(ef_core_cosmos_db_where_embedded_navigation_property_null));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Item>(entity => entity.OwnsOne(item => item.Metadata));
    }
  }

  class Item
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Metadata Metadata { get; set; }
  }

  class Metadata
  {
    public Guid Id { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
  }
}
