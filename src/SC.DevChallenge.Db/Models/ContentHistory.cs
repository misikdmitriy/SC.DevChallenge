using System;

namespace SC.DevChallenge.Db.Models
{
    public class ContentHistory
    {
        public int Id { get; set; }
        public byte[] Hash { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
