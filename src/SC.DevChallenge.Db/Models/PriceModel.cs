using System;
using SC.DevChallenge.Db.Models.Contracts;

namespace SC.DevChallenge.Db.Models
{
    public class PriceModel : IIDentifiable
    {
        public int Id { get; set; }
        public Portfolio Portfolio { get; set; }
        public int PortfolioId { get; set; }
        public InstrumentOwner InstrumentOwner { get; set; }
        public int InstrumentOwnerId { get; set; }
        public Instrument Instrument { get; set; }
        public int InstrumentId { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
    }
}
