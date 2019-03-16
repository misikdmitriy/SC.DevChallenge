using System;
using System.Globalization;
using SC.DevChallenge.Core.Services.Contracts;
using SC.DevChallenge.Db.Models;

namespace SC.DevChallenge.Core.Services
{
    public class PriceModelParser : IPriceModelParser
    {
        public PriceModel ParseCsv(string line)
        {
            var separated = line.Split(';');

            if (separated.Length < 5)
            {
                throw new ArgumentException("Provided CSV cannot be parsed to PriceModel", 
                    nameof(line));
            }

            return new PriceModel
            {
                Portfolio = new Portfolio
                {
                    Name = separated[0]
                },
                InstrumentOwner = new InstrumentOwner
                {
                    Name = separated[1]
                },
                Instrument = new Instrument
                {
                    Name = separated[2]
                },
                Date = DateTime.Parse(separated[3], CultureInfo.InvariantCulture),
                Price = decimal.Parse(separated[4], CultureInfo.InvariantCulture)
            };
        }
    }
}
