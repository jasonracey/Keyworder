namespace Keyworder.Utilities;

using System.Threading.Tasks;
using Microsoft.JSInterop;

public sealed class ClipboardService
{
    private readonly IJSRuntime jsRuntime;

    public ClipboardService(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public ValueTask WriteTextAsync(string text)
    {
        return jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
    }
}