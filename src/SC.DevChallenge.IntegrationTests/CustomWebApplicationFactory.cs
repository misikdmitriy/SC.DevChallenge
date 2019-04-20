using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SC.DevChallenge.Db.Contexts;
using SC.DevChallenge.Db.Models;

namespace SC.DevChallenge.IntegrationTests
{
	public class CustomWebApplicationFactory<TStartup>
		: WebApplicationFactory<TStartup> where TStartup : class
	{
		protected override IWebHostBuilder CreateWebHostBuilder()
		{
			return WebHost.CreateDefaultBuilder()
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseStartup<TStartup>();
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureServices(services =>
			{
				// Add a database context (ApplicationDbContext) using an in-memory 
				// database for testing.
				services.AddDbContext<AppDbContext>(options =>
				{
					options.UseSqlite("Data Source=test.db");
				});

				// Build the service provider.
				var sp = services.BuildServiceProvider();

				// Create a scope to obtain a reference to the database
				// context (ApplicationDbContext).
				using (var scope = sp.CreateScope())
				{
					var scopedServices = scope.ServiceProvider;
					var db = scopedServices.GetRequiredService<AppDbContext>();

					CreateTestDb(db);
				}
			});
		}

		private static void CreateTestDb(AppDbContext db)
		{
			// Ensure the database is created.
			db.Database.EnsureCreated();

			// Remove all data
			db.InstrumentOwners.RemoveRange(db.InstrumentOwners);
			db.Instruments.RemoveRange(db.Instruments);
			db.Portfolios.RemoveRange(db.Portfolios);
			db.PriceModels.RemoveRange(db.PriceModels);
			db.SaveChanges();

			// Insert test data
			var portfolios = new[]
			{
				new Portfolio {Name = "portfolio1"},
				new Portfolio {Name = "portfolio2"},
			};
			var owners = new[]
			{
				new InstrumentOwner {Name = "owner1"},
				new InstrumentOwner {Name = "owner2"},
				new InstrumentOwner {Name = "owner3"},
            };
			var instruments = new[]
			{
				new Instrument {Name = "instrument1"},
				new Instrument {Name = "instrument2"},
				new Instrument {Name = "instrument3"},
            };

			db.Portfolios.AddRange(portfolios);
			db.InstrumentOwners.AddRange(owners);
			db.Instruments.AddRange(instruments);

			db.SaveChanges();

			var priceModels = new[]
			{
				new PriceModel
				{
					InstrumentId = instruments[0].Id,
					InstrumentOwnerId = owners[0].Id,
					PortfolioId = portfolios[0].Id,
					Date = new DateTime(2018, 1, 1, 0, 0, 0),
					Price = 1.00m
				},
                new PriceModel
                {
                    InstrumentId = instruments[2].Id,
                    InstrumentOwnerId = owners[2].Id,
                    PortfolioId = portfolios[0].Id,
                    Date = new DateTime(2018, 1, 1, 1, 0, 0),
                    Price = 2.00m
                },
                new PriceModel
                {
                    InstrumentId = instruments[2].Id,
                    InstrumentOwnerId = owners[2].Id,
                    PortfolioId = portfolios[0].Id,
                    Date = new DateTime(2018, 1, 1, 1, 5, 0),
                    Price = 20.00m
                },
                new PriceModel
                {
                    InstrumentId = instruments[2].Id,
                    InstrumentOwnerId = owners[2].Id,
                    PortfolioId = portfolios[0].Id,
                    Date = new DateTime(2018, 1, 1, 1, 6, 0),
                    Price = 15.00m
                },
                new PriceModel
                {
                    InstrumentId = instruments[2].Id,
                    InstrumentOwnerId = owners[2].Id,
                    PortfolioId = portfolios[0].Id,
                    Date = new DateTime(2018, 1, 1, 1, 7, 0),
                    Price = 150.00m
                },
                new PriceModel
				{
					InstrumentId = instruments[0].Id,
					InstrumentOwnerId = owners[0].Id,
					PortfolioId = portfolios[0].Id,
					Date = new DateTime(2018, 1, 1, 0, 0, 0),
					Price = 1.00m
				},
				new PriceModel
				{
					InstrumentId = instruments[0].Id,
					InstrumentOwnerId = owners[0].Id,
					PortfolioId = portfolios[0].Id,
					Date = new DateTime(2018, 1, 1, 0, 0, 0),
					Price = 2.00m
				},
				new PriceModel
				{
					InstrumentId = instruments[1].Id,
					InstrumentOwnerId = owners[0].Id,
					PortfolioId = portfolios[0].Id,
					Date = new DateTime(2018, 1, 1, 0, 0, 0),
					Price = 2.00m
				},
				new PriceModel
				{
					InstrumentId = instruments[0].Id,
					InstrumentOwnerId = owners[1].Id,
					PortfolioId = portfolios[0].Id,
					Date = new DateTime(2018, 1, 1, 0, 0, 0),
					Price = 2.00m
				},
				new PriceModel
				{
					InstrumentId = instruments[0].Id,
					InstrumentOwnerId = owners[0].Id,
					PortfolioId = portfolios[1].Id,
					Date = new DateTime(2018, 1, 1, 0, 0, 0),
					Price = 2.00m
				},
			};

			db.PriceModels.AddRange(priceModels);

			db.SaveChanges();
		}
	}
}
