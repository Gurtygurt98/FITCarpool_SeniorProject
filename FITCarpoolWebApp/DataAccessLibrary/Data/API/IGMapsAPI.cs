namespace DataAccessLibrary.Data.API
{
    public interface IGMapsAPI
    {
        string MapAPI { get; }
        Task<string> GetLocationInfoAsync(double latitude, double longitude);
    }
}