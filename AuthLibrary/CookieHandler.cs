using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.Extensions.Logging;

namespace AuthLibrary;

public class CookieHandler(ILogger<CookieHandler> logger) : DelegatingHandler
{
	private readonly ILogger<CookieHandler> _logger = logger;

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		_logger.LogDebug("Using CookieHandler");
		request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
		return await base.SendAsync(request, cancellationToken);
	}
}
