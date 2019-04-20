using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using SC.DevChallenge.Core.Exceptions;
using SC.DevChallenge.Core.Services;
using SC.DevChallenge.Core.Services.Contracts;
using SC.DevChallenge.Db.Contexts;
using SC.DevChallenge.Db.Factories;
using SC.DevChallenge.Db.Models;
using Shouldly;
using Xunit;

namespace SC.DevChallenge.UnitTests
{
    public class PriceModelServiceTests : IDisposable
    {
        private readonly IPriceModelService _service;
        private readonly Mock<IDateTimeConverter> _converterMock;
        private readonly AppDbContextFactory _factory;

        public PriceModelServiceTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlite("Data Source=test.db");
            _factory = new AppDbContextFactory(optionsBuilder.Options);

            _converterMock = new Mock<IDateTimeConverter>();

            _service = new PriceModelService(_factory, _converterMock.Object);

            using (var context = _factory.CreateContext())
            {
                InitDbData(context);
            }
        }

        [Fact]
        public async Task GetAverageShouldReturnAverageBetweenModels()
        {
            // Arrange
            const int timeSlot = 1;
            var start = new DateTime(2018, 1, 1, 0, 0, 0, 0);
            _converterMock.Setup(x => x.GetTimeSlotStartDate(timeSlot))
                .Returns(start);

            _converterMock.Setup(x => x.GetTimeSlotStartDate(timeSlot + 1))
                .Returns(start.AddDays(1));
            
            // Act
            var average = await _service.GetAverage(timeSlot, "owner1", "instrument1",
                "portfolio1");

            // Assert
            average.Price.ShouldBe(2.0m);
            average.Start.ShouldBe(start);
        }

        [Fact]
        public async Task GetAverageShouldReturnAverageBetweenModels2()
        {
            // Arrange
            const int timeSlot = 1;
            var start = new DateTime(2018, 1, 1, 0, 0, 0, 0);
            _converterMock.Setup(x => x.GetTimeSlotStartDate(timeSlot))
                .Returns(start);

            _converterMock.Setup(x => x.GetTimeSlotStartDate(timeSlot + 1))
                .Returns(start.AddDays(2));
            
            // Act
            var average = await _service.GetAverage(timeSlot, "owner1", "instrument1",
                "portfolio1");

            // Assert
            average.Price.ShouldBe(2.5m);
            average.Start.ShouldBe(start);
        }

        [Fact]
        public async Task GetAverageShouldThrowExceptionIfModelsAbsent()
        {
            // Arrange
            const int timeSlot = 1;
            var start = new DateTime(2018, 1, 1, 0, 0, 0, 0);
            _converterMock.Setup(x => x.GetTimeSlotStartDate(timeSlot))
                .Returns(start);

            _converterMock.Setup(x => x.GetTimeSlotStartDate(timeSlot + 1))
                .Returns(start.AddDays(2));

            // Act
            try
            {
                // Assert
                await _service.GetAverage(timeSlot, Guid.NewGuid().ToString());
            }
            catch (PriceModelAbsentException)
            {
                // all ok
            }
        }

        [Fact]
        public async Task GetBenchmarkShouldReturnAverageBetweenModels()
        {
            // Arrange
            const int timeSlot = 1;
            var start = new DateTime(2018, 1, 1, 0, 0, 0, 0);
            _converterMock.Setup(x => x.GetTimeSlotStartDate(timeSlot))
                .Returns(start);

            _converterMock.Setup(x => x.GetTimeSlotStartDate(timeSlot + 1))
                .Returns(start.AddDays(1));

            // Act
            var average = await _service.GetBenchmark(timeSlot, "owner1", "instrument1",
                "portfolio1");

            // Assert
            average.Price.ShouldBe(3.0m);
            average.Start.ShouldBe(start);
        }

        [Fact]
        public async Task GetBenchmarkShouldReturnAverageBetweenModels2()
        {
            // Arrange
            const int timeSlot = 1;
            var start = new DateTime(2018, 1, 1, 0, 0, 0, 0);
            _converterMock.Setup(x => x.GetTimeSlotStartDate(timeSlot))
                .Returns(start);

            _converterMock.Setup(x => x.GetTimeSlotStartDate(timeSlot + 1))
                .Returns(start.AddDays(2));

            // Act
            var average = await _service.GetBenchmark(timeSlot, "owner1", "instrument1",
                "portfolio1");

            // Assert
            average.Price.ShouldBe(2.5m);
            average.Start.ShouldBe(start);
        }

        [Fact]
        public async Task GetBenchmarkShouldReturnAverageBetweenModels3()
        {
            // Arrange
            const int timeSlot = 1;
            var start = new DateTime(2018, 1, 1, 0, 0, 0, 0);
            _converterMock.Setup(x => x.GetTimeSlotStartDate(timeSlot))
                .Returns(start);

            _converterMock.Setup(x => x.GetTimeSlotStartDate(timeSlot + 1))
                .Returns(start.AddMonths(1));

            // Act
            var average = await _service.GetBenchmark(timeSlot, "owner1", "instrument1",
                "portfolio1");

            // Assert
            average.Price.ShouldBe(3.5m);
            average.Start.ShouldBe(start);
        }

        [Fact]
        public async Task GetBenchmarkShouldThrowExceptionIfModelsAbsent()
        {
            // Arrange
            const int timeSlot = 1;
            var start = new DateTime(2018, 1, 1, 0, 0, 0, 0);
            _converterMock.Setup(x => x.GetTimeSlotStartDate(timeSlot))
                .Returns(start);

            _converterMock.Setup(x => x.GetTimeSlotStartDate(timeSlot + 1))
                .Returns(start.AddDays(2));

            // Act
            try
            {
                // Assert
                await _service.GetBenchmark(timeSlot, Guid.NewGuid().ToString());
            }
            catch (PriceModelAbsentException)
            {
                // all ok
            }
        }

        public void Dispose()
        {
            using (var context = _factory.CreateContext())
            {
                context.PriceModels.RemoveRange(context.PriceModels);
                context.SaveChanges();
            }
        }

        private void InitDbData(AppDbContext db)
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
            };
            var owners = new[]
            {
                new InstrumentOwner {Name = "owner1"},
            };
            var instruments = new[]
            {
                new Instrument {Name = "instrument1"},
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
                    InstrumentId = instruments[0].Id,
                    InstrumentOwnerId = owners[0].Id,
                    PortfolioId = portfolios[0].Id,
                    Date = new DateTime(2018, 1, 2, 0, 0, 0),
                    Price = 2.00m
                },
                new PriceModel
                {
                    InstrumentId = instruments[0].Id,
                    InstrumentOwnerId = owners[0].Id,
                    PortfolioId = portfolios[0].Id,
                    Date = new DateTime(2018, 1, 1, 0, 0, 0),
                    Price = 3.00m
                },
                new PriceModel
                {
                    InstrumentId = instruments[0].Id,
                    InstrumentOwnerId = owners[0].Id,
                    PortfolioId = portfolios[0].Id,
                    Date = new DateTime(2018, 1, 2, 0, 0, 0),
                    Price = 4.00m
                },
                new PriceModel
                {
                    InstrumentId = instruments[0].Id,
                    InstrumentOwnerId = owners[0].Id,
                    PortfolioId = portfolios[0].Id,
                    Date = new DateTime(2018, 1, 3, 0, 0, 0),
                    Price = 5.00m
                },
                new PriceModel
                {
                    InstrumentId = instruments[0].Id,
                    InstrumentOwnerId = owners[0].Id,
                    PortfolioId = portfolios[0].Id,
                    Date = new DateTime(2018, 1, 4, 0, 0, 0),
                    Price = 6.00m
                },
            };

            db.PriceModels.AddRange(priceModels);

            db.SaveChanges();
        }
    }
}
