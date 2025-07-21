using Microsoft.JSInterop;

namespace ClientLibrary.Services.Helpers;

public class JsLogger
{
    private readonly IJSRuntime _jsRuntime;

    public JsLogger(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task Log(string message)
    {
        await _jsRuntime.InvokeVoidAsync("logToConsole", message);
    }
}
