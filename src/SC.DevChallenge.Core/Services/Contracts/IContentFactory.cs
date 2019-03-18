namespace SC.DevChallenge.Core.Services.Contracts
{
    public interface IContentFactory
    {
	    bool IsUpdateRequired();
        void ParseContentFromCsv(string filepath);
    }
}
