using Coravel.Invocable;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Diagnostics;

namespace Serilog.Components;

public class SqlServerCleanup(string connectionString, IOptions<SerilogOptions> options, ILogger<SqlServerCleanup> logger) : IInvocable, ICancellableInvocable
{
	private readonly string _connectionString = connectionString;
	private readonly SerilogOptions _options = options.Value;
	private readonly ILogger<SqlServerCleanup> _logger = logger;

	public CancellationToken CancellationToken { get; set; }

	public async Task Invoke()
	{
		_logger.LogDebug("Deleting logs older than {RetainDays} days", _options.RetainDays);

		using var cn = new SqlConnection(_connectionString);
		var deleted = 0;

		try
		{
			var sw = Stopwatch.StartNew();
			do
			{
				// delete in small chunks to avoid locking
				var chunk = await DeleteTopAsync(cn, _options.Schema, _options.TableName, _options.RetainDays, _options.DeleteChunkSize);
				deleted += chunk;
				if (chunk == 0) break;
			} while (!CancellationToken.IsCancellationRequested);
			sw.Stop();
			_logger.LogInformation("Deleted {Deleted} logs in {Elapsed} ms", deleted, sw.ElapsedMilliseconds);
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error deleting logs");
		}
	}

	private static async Task<int> DeleteTopAsync(IDbConnection connection, string schema, string tableName, int retainDays, int chunkSize)
	{
		var sql = $@"DELETE TOP ({chunkSize}) FROM [{schema}].[{tableName}] WHERE [Timestamp] < DATEADD(DAY, -{retainDays}, GETUTCDATE())";

		// deletes can be very slow, so ample timeout is added
		return await connection.ExecuteAsync(sql, commandTimeout: 90);
	}
}
