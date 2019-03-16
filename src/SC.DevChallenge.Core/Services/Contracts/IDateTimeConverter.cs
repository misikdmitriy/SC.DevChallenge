using System;

namespace SC.DevChallenge.Core.Services.Contracts
{
    public interface IDateTimeConverter
    {
        int DateTimeToTimeSlot(DateTime dateTime);
        DateTime GetTimeSlotStartDate(int timeSlot);
    }
}
