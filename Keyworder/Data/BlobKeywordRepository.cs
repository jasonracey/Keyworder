using System.Text;
using Azure.Storage.Blobs;
using Newtonsoft.Json;

namespace Keyworder.Data;

public class BlobKeywordRepository : IKeywordRepository
{
    private const string BlobContainerName = "keyworder-data";
    private const string BlobName = "Keywords.json";

    private readonly BlobClient _blobClient;

    public BlobKeywordRepository(string storageAccountConnectionString)
    {
        if (string.IsNullOrWhiteSpace(storageAccountConnectionString)) throw new ArgumentNullException(nameof(storageAccountConnectionString));
        
        var blobContainerClient = new BlobContainerClient(storageAccountConnectionString, BlobContainerName);
        _blobClient = blobContainerClient.GetBlobClient(BlobName);
    }
    
    public async Task<IEnumerable<Keyword>> ReadAsync()
    {
        var response = await _blobClient.DownloadAsync().ConfigureAwait(false);
        using var streamReader = new StreamReader(response.Value.Content);
        var builder = new StringBuilder();
        while (!streamReader.EndOfStream)
        {
            var line = await streamReader.ReadLineAsync().ConfigureAwait(false);
            if (line != null) builder.AppendLine(line);
        }
        var json = builder.ToString();
        return JsonConvert.DeserializeObject<IEnumerable<Keyword>>(json) ?? Enumerable.Empty<Keyword>();
    }

    public async Task<IEnumerable<Keyword>> WriteAsync(IEnumerable<Keyword> keywords)
    {
        var json = JsonConvert.SerializeObject(keywords);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        await _blobClient.DeleteIfExistsAsync();
        await _blobClient.UploadAsync(stream).ConfigureAwait(false);
        return await ReadAsync().ConfigureAwait(false);
    }
}