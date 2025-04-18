namespace GAC.WMS.Integration.Domain.Interfaces
{
    public interface IFileParser
    {
        Task<T> ParseAsync<T>(string filePath);
    }
}
