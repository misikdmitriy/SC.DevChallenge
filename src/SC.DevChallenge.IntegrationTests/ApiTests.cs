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
		public async Task ApiShouldReturnCorrectModel1()
		{
			// Arrange
			var expected = new AveragePriceModel
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
				var result = JsonConvert.DeserializeObject<AveragePriceModel>(content);

				result.Date.ShouldBe(expected.Date);
				result.Price.ShouldBe(expected.Price);
			}
		}

		[Fact]
		public async Task ApiShouldReturnCorrectModel2()
		{
			// Arrange
			var expected = new AveragePriceModel
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
				var result = JsonConvert.DeserializeObject<AveragePriceModel>(content);

				result.Date.ShouldBe(expected.Date);
				result.Price.ShouldBe(expected.Price);
			}
		}

		[Fact]
		public async Task ApiShouldReturnBadRequestInCaseIfDateIsIncorrect()
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
		public async Task ApiShouldReturnNotFoundIfNoPriceForThisPeriod()
		{
			// Arrange
			// Act
			using (var response = await _client.GetAsync("api/prices/average?portfolio=portfolio2&owner=owner1&instrument=instrument1&date=01%2F01%2F2020%2000%3A00%3A00"))
			{
				// Assert
				response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
			}
		}
	}
}
