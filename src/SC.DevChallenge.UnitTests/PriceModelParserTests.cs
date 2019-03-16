using System;
using System.Globalization;
using SC.DevChallenge.Core.Services;
using SC.DevChallenge.Core.Services.Contracts;
using Shouldly;
using Xunit;

namespace SC.DevChallenge.UnitTests
{
    public class PriceModelParserTests
    {
        private readonly IPriceModelParser _parser;

        public PriceModelParserTests()
        {
            _parser = new PriceModelParser();
        }

        [Theory]
        [InlineData("Microsoft", "Fannie Mae", "Deposit", "2018-03-15T17:33:40", "3.00")]
        [InlineData("Google", "Another", "NotDeposit", "2018-03-15T17:35:00", "5.00")]
        public void ParseCsvShouldReturnCorrectModel(string owner, string portfolio, 
            string instrument, string date, string price)
        {
            // Arrange
            var line = $"{portfolio};{owner};{instrument};{date};{price}";

            // Act
            var result = _parser.ParseCsv(line);

            // Assert
            result.InstrumentOwner.Name.ShouldBe(owner);
            result.Portfolio.Name.ShouldBe(portfolio);
            result.Instrument.Name.ShouldBe(instrument);
            result.Price.ShouldBe(decimal.Parse(price, CultureInfo.InvariantCulture));
            result.Date.ShouldBe(DateTime.Parse(date, CultureInfo.InvariantCulture));
        }
    }
}
