using System.Text;
using Azure.Identity;
using Azure.Storage.Blobs;
using Newtonsoft.Json;

namespace Keyworder.Data;

public class BlobKeywordRepository : IKeywordRepository
{
    private static readonly Uri BlobContainerUri = new("https://keyworder.blob.core.windows.net/keyworder-data");
    private static readonly BlobContainerClient BlobContainerClient = new (BlobContainerUri, new DefaultAzureCredential());
    private static readonly BlobClient BlobClient = BlobContainerClient.GetBlobClient("Keywords.json");

    public async Task<IEnumerable<Keyword>> ReadAsync()
    {
        var response = await BlobClient.DownloadAsync().ConfigureAwait(false);
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
        var _ = await BlobClient.UploadAsync(stream).ConfigureAwait(false);
        return await ReadAsync().ConfigureAwait(false);
    }
}