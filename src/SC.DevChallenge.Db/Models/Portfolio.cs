using SC.DevChallenge.Db.Models.Contracts;

namespace SC.DevChallenge.Db.Models
{
    public class Portfolio : IIDentifiable
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
