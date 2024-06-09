using Coravel.Invocable;

namespace Application.Features.Serilog;

/// <summary>
/// intended to extract query and param details from log entries for easier analysis
/// </summary>
internal class AsyncEnrich(string connectionString) : IInvocable
{
	private readonly string _connectionString = connectionString;

	public Task Invoke() => throw new NotImplementedException();
}
