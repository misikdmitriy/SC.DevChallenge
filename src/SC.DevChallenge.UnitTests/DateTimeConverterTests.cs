using System;
using SC.DevChallenge.Core.Services;
using SC.DevChallenge.Core.Services.Contracts;
using Shouldly;
using Xunit;

namespace SC.DevChallenge.UnitTests
{
    public class DateTimeConverterTests
    {
        private readonly IDateTimeConverter _converter;

        public DateTimeConverterTests()
        {
            _converter = new DateTimeConverter();
        }

        [Fact]
        public void DateTimeToTimeSlotShouldReturnCorrectTimeSlot1()
        {
            // Arrange
            var dateTime = new DateTime(2018, 1, 1, 0, 0, 0);
            
            // Act
            var interval = _converter.DateTimeToTimeSlot(dateTime);

            // Assert
            interval.ShouldBe(0);
        }

        [Fact]
        public void DateTimeToTimeSlotShouldReturnCorrectTimeSlot2()
        {
            // Arrange
            var dateTime = new DateTime(2018, 1, 1, 0, 0, 1);

            // Act
            var interval = _converter.DateTimeToTimeSlot(dateTime);

            // Assert
            interval.ShouldBe(0);
        }

        [Fact]
        public void DateTimeToTimeSlotShouldReturnCorrectTimeSlot3()
        {
            // Arrange
            var dateTime = new DateTime(2018, 1, 1, 0, 0, 0)
                .AddSeconds(10000);

            // Act
            var interval = _converter.DateTimeToTimeSlot(dateTime);

            // Assert
            interval.ShouldBe(1);
        }

        [Fact]
        public void DateTimeToTimeSlotShouldReturnCorrectTimeSlot4()
        {
            // Arrange
            var dateTime = new DateTime(2018, 1, 1, 0, 0, 0)
                .AddSeconds(20000);

            // Act
            var interval = _converter.DateTimeToTimeSlot(dateTime);

            // Assert
            interval.ShouldBe(2);
        }


        [Fact]
        public void DateTimeToTimeSlotShouldReturnCorrectTimeSlot5()
        {
            // Arrange
            var dateTime = new DateTime(2018, 1, 1, 0, 0, 0)
                .AddSeconds(9999);

            // Act
            var interval = _converter.DateTimeToTimeSlot(dateTime);

            // Assert
            interval.ShouldBe(0);
        }

        [Fact]
        public void DateTimeToTimeSlotShouldFailIfProvidedDateIsTooEarly()
        {
            // Arrange
            var dateTime = new DateTime(2017, 12, 31, 23, 59, 59);

            // Act
            Action act = () => _converter.DateTimeToTimeSlot(dateTime);

            // Assert
            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void GetTimeSlotStartDateShouldReturnCorrectStartDate1()
        {
            // Arrange
            const int timeSlot = 0;
            var expected = new DateTime(2018, 1, 1, 0, 0, 0);

            // Act
            var dateTime = _converter.GetTimeSlotStartDate(timeSlot);

            // Assert
            dateTime.ShouldBe(expected);
        }

        [Fact]
        public void GetTimeSlotStartDateShouldReturnCorrectStartDate2()
        {
            // Arrange
            const int timeSlot = 1;
            var expected = new DateTime(2018, 1, 1, 0, 0, 0)
                .AddSeconds(10000);

            // Act
            var dateTime = _converter.GetTimeSlotStartDate(timeSlot);

            // Assert
            dateTime.ShouldBe(expected);
        }

        [Fact]
        public void GetTimeSlotStartDateShouldThrowExceptionForNegative()
        {
            // Arrange
            const int timeSlot = -1;

            // Act
            Action act = () => _converter.GetTimeSlotStartDate(timeSlot);

            // Assert
            act.ShouldThrow<ArgumentOutOfRangeException>();
        }
    }
}
