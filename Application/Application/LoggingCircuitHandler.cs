using Microsoft.AspNetCore.Components.Server.Circuits;

namespace Application;

public class LoggingCircuitHandler : CircuitHandler
{
	private readonly ILogger<LoggingCircuitHandler> _logger;

	public LoggingCircuitHandler(ILogger<LoggingCircuitHandler> logger) => _logger = logger;

	public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
	{
		_logger.LogDebug("Circuit opened");
		return base.OnCircuitOpenedAsync(circuit, cancellationToken);
	}

	public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
	{
		_logger.LogDebug("Circuit closed");
		return base.OnCircuitClosedAsync(circuit, cancellationToken);
	}

	public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
	{
		_logger.LogDebug("Connection down");
		return base.OnConnectionDownAsync(circuit, cancellationToken);
	}
}
