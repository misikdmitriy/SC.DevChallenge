using System;
using SC.DevChallenge.Core.Extensions;
using Shouldly;
using Xunit;

namespace SC.DevChallenge.UnitTests
{
    public class StatisticExtensionsTests
    {
        [Fact]
        public void CountBenchmarkShouldDoItForCollection1()
        {
            // Arrange
            var collection = new[] { 1m, 50m, 100m, 1000m, 10000m };

            // Act
            var result = collection.CountBenchmark();

            // Assert
            result.ShouldBe(287.75m);
        }

        [Fact]
        public void CountBenchmarkShouldDoItForCollection2()
        {
            // Arrange
            var collection = new[] { 1m };

            // Act
            var result = collection.CountBenchmark();

            // Assert
            result.ShouldBe(1m);
        }


        [Fact]
        public void CountBenchmarkShouldFailForEmpty()
        {
            // Arrange
            var collection = new decimal[0];

            // Act
            Action act = () => collection.CountBenchmark();

            // Assert
            act.ShouldThrow<ArgumentNullException>();
        }
    }
}
