using System;
using SC.DevChallenge.Core.Services.Contracts;

namespace SC.DevChallenge.Core.Services
{
    public class DateTimeConverter : IDateTimeConverter
    {
        private readonly DateTime _start = new DateTime(2018, 1, 1, 0, 0, 0);
        private readonly TimeSpan _timeSlotDuration = TimeSpan.FromSeconds(10000);

        public int DateTimeToTimeSlot(DateTime dateTime)
        {
            if (dateTime < _start)
            {
                throw new ArgumentOutOfRangeException(nameof(dateTime), dateTime,
                    $"Provided date should be after {_start}");
            }

            var delta = dateTime - _start;
            var timeSlotNumber = delta.Ticks / _timeSlotDuration.Ticks;

            if (timeSlotNumber > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(dateTime), dateTime,
                    $"Provided date is too large");
            }

            return (int)timeSlotNumber;
        }

        public DateTime GetTimeSlotStartDate(int timeSlot)
        {
            if (timeSlot < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(timeSlot), timeSlot,
                    $"timeSlot cannot be negative");
            }

            var delta = (double)_timeSlotDuration.Ticks * timeSlot;

            if (delta > long.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(timeSlot), timeSlot,
                    $"Provided timeSlot is too large");
            }

            return _start.Add(TimeSpan.FromTicks((long)delta));
        }
    }
}
