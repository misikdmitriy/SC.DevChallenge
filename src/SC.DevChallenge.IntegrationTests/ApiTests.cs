using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SC.DevChallenge.Api;
using SC.DevChallenge.IntegrationTests.Models;
using Shouldly;
using Xunit;

namespace SC.DevChallenge.IntegrationTests
{
    public class ApiTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public ApiTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task AverageApiShouldReturnCorrectModel1()
        {
            // Arrange
            var expected = new TestPriceModel
            {
                Price = 1.33m,
                Date = new DateTime(2018, 1, 1, 0, 0, 0)
            };

            // Act
            using (var response = await _client.GetAsync("api/prices/average?portfolio=portfolio1&owner=owner1&instrument=instrument1&date=01%2F01%2F2018%2000%3A00%3A00"))
            {
                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.OK);

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TestPriceModel>(content);

                result.Date.ShouldBe(expected.Date);
                result.Price.ShouldBe(expected.Price);
            }
        }

        [Fact]
        public async Task AverageApiShouldReturnCorrectModel2()
        {
            // Arrange
            var expected = new TestPriceModel
            {
                Price = 2.00m,
                Date = new DateTime(2018, 1, 1, 0, 0, 0)
            };

            // Act
            using (var response = await _client.GetAsync("api/prices/average?portfolio=portfolio2&owner=owner1&instrument=instrument1&date=01%2F01%2F2018%2000%3A00%3A00"))
            {
                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.OK);

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TestPriceModel>(content);

                result.Date.ShouldBe(expected.Date);
                result.Price.ShouldBe(expected.Price);
            }
        }

        [Fact]
        public async Task AverageApiShouldReturnBadRequestInCaseIfDateIsIncorrect()
        {
            // Arrange
            // Act
            using (var response = await _client.GetAsync("api/prices/average?portfolio=portfolio2&owner=owner1&instrument=instrument1&date=01%2F01%2F2017%2000%3A00%3A00"))
            {
                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task AverageApiShouldReturnNotFoundIfNoPriceForThisPeriod()
        {
            // Arrange
            // Act
            using (var response = await _client.GetAsync("api/prices/average?portfolio=portfolio2&owner=owner1&instrument=instrument1&date=01%2F01%2F2020%2000%3A00%3A00"))
            {
                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public async Task AverageApiShouldReturnBadRequestIfNoFilterProvided()
        {
            // Arrange
            // Act
            using (var response = await _client.GetAsync("api/prices/average?date=01%2F01%2F2018%2000%3A00%3A00"))
            {
                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task AverageApiShouldReturnBadRequestIfDateNotProvided()
        {
            // Arrange
            // Act
            using (var response = await _client.GetAsync("api/prices/average?portfolio=portfolio2&owner=owner1&instrument=instrument1"))
            {
                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task BenchmarkApiShouldReturnCorrectModel()
        {
            // Arrange
            var expected = new TestPriceModel
            {
                Price = 5.63m,
                Date = new DateTime(2018, 1, 1, 0, 0, 0)
            };

            // Act
            using (var response = await _client.GetAsync("api/prices/benchmark?portfolio=portfolio1&date=01%2F01%2F2018%2000%3A00%3A00"))
            {
                response.StatusCode.ShouldBe(HttpStatusCode.OK);

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TestPriceModel>(content);

                result.Date.ShouldBe(expected.Date);
                result.Price.ShouldBe(expected.Price);
            }
        }

        [Fact]
        public async Task BenchmarkApiShouldReturnBadRequestInCaseIfDateIsIncorrect()
        {
            // Arrange
            // Act
            using (var response = await _client.GetAsync("api/prices/benchmark?portfolio=portfolio1&date=01%2F01%2F2017%2000%3A00%3A00"))
            {
                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task BenchmarkApiShouldReturnNotFoundIfNoPriceForThisPeriod()
        {
            // Arrange
            // Act
            using (var response = await _client.GetAsync("api/prices/benchmark?portfolio=portfolio1&date=01%2F01%2F2020%2000%3A00%3A00"))
            {
                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public async Task BenchmarkApiShouldReturnBadRequestIfPortfolioIsNotProvided()
        {
            // Arrange
            // Act
            using (var response = await _client.GetAsync("api/prices/benchmark?date=01%2F01%2F2018%2000%3A00%3A00"))
            {
                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task AggregateApiShouldReturnCorrectModel()
        {
            // Arrange
            var expected = new[]
            {
                new TestPriceModel
                {
                    Date = new DateTime(2018, 1, 1, 0, 0, 0),
                    Price = 5.63m
                },
                new TestPriceModel
                {
                    Date = new DateTime(2018, 1, 1, 2, 46, 40),
                    Price = 250m
                }
            };

            // Act
            using (var response = await _client.GetAsync("api/prices/aggregate?portfolio=portfolio1&startdate=01%2F01%2F2018%2000%3A00%3A00&" +
                                                         "enddate=01%2F01%2F2018%2004%3A00%3A00&resultpoints=2"))
            {
                response.StatusCode.ShouldBe(HttpStatusCode.OK);

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TestPriceModel[]>(content);

                result.Length.ShouldBe(expected.Length);

                for (var i = 0; i < result.Length; i++)
                {
                    result[i].Price.ShouldBe(expected[i].Price);
                    result[i].Date.ShouldBe(expected[i].Date);
                }
            }
        }

        [Fact]
        public async Task AggregateApiShouldReturnBadRequestInCaseIfStartDateIsIncorrect()
        {
            // Arrange
            // Act
            using (var response = await _client.GetAsync("api/prices/aggregate?portfolio=portfolio1&startdate=01%2F01%2F2017%2000%3A00%3A00&" +
                                                         "enddate=01%2F01%2F2018%2000%3A40%3A00&resultpoints=2"))
            {
                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task AggregateApiShouldReturnBadRequestInCaseIfEndDateIsIncorrect()
        {
            // Arrange
            // Act
            using (var response = await _client.GetAsync("api/prices/aggregate?portfolio=portfolio1&startdate=01%2F01%2F2018%2000%3A00%3A00&" +
                                                         "enddate=01%2F01%2F2017%2000%3A40%3A00&resultpoints=2"))
            {
                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            }
        }


        [Fact]
        public async Task AggregateApiShouldReturnBadRequestInCaseIfEndDateBeforeStartDate()
        {
            // Arrange
            // Act
            using (var response = await _client.GetAsync("api/prices/aggregate?portfolio=portfolio1&startdate=01%2F02%2F2018%2000%3A00%3A00&" +
                                                         "enddate=01%2F01%2F2018%2000%3A40%3A00&resultpoints=2"))
            {
                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task AggregateApiShouldReturnNotFoundIfNoPriceForThisPeriod()
        {
            // Arrange
            // Act
            using (var response = await _client.GetAsync("api/prices/aggregate?portfolio=portfolio1&startdate=01%2F02%2F2018%2000%3A00%3A00&" +
                                                         "enddate=01%2F03%2F2018%2000%3A40%3A00&resultpoints=2"))
            {
                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public async Task AggregateApiShouldReturnBadRequestIfPortfolioIsNotProvided()
        {
            // Arrange
            // Act
            using (var response = await _client.GetAsync("api/prices/aggregate?startdate=01%2F01%2F2018%2000%3A00%3A00&" +
                                                         "enddate=01%2F01%2F2018%2000%3A40%3A00&resultpoints=2"))
            {
                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task AggregateApiShouldReturnBadRequestIfResultPointIsZero()
        {
            // Arrange
            // Act
            using (var response = await _client.GetAsync("api/prices/aggregate?portfolio=portfolio1&startdate=01%2F01%2F2018%2000%3A00%3A00&" +
                                                         "enddate=01%2F01%2F2018%2004%3A00%3A00&resultpoints=0"))
            {
                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task AggregateApiShouldReturnBadRequestIfResultPointIsNegative()
        {
            // Arrange
            // Act
            using (var response = await _client.GetAsync("api/prices/aggregate?portfolio=portfolio1&startdate=01%2F01%2F2018%2000%3A00%3A00&" +
                                                         "enddate=01%2F01%2F2018%2004%3A00%3A00&resultpoints=-1"))
            {
                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task AggregateApiShouldReturnBadRequestIfResultPointsMoreThanTimeSlots()
        {
            // Arrange
            // Act
            using (var response = await _client.GetAsync("api/prices/aggregate?portfolio=portfolio1&startdate=01%2F01%2F2018%2000%3A00%3A00&" +
                                                         "enddate=01%2F01%2F2018%2004%3A00%3A00&resultpoints=1000"))
            {
                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task AggregateApiShouldReturnBadRequestIfShortPeriodProvided()
        {
            // Arrange
            // Act
            using (var response = await _client.GetAsync("api/prices/aggregate?portfolio=portfolio1&startdate=01%2F01%2F2018%2000%3A00%3A00&" +
                                                         "enddate=01%2F01%2F2018%2000%3A01%3A00&resultpoints=1"))
            {
                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            }
        }
    }
}
