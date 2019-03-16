using SC.DevChallenge.Db.Models;

namespace SC.DevChallenge.Core.Services.Contracts
{
    public interface IPriceModelParser
    {
        PriceModel ParseCsv(string line);
    }
}
