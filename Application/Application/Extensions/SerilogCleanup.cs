using Coravel.Invocable;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace Application.Extensions;

internal class SerilogCleanup(string connectionString, string schema, string tableName, int retainDays, ILogger<SerilogCleanup> logger) : IInvocable, ICancellableInvocable
{
	private readonly string _connectionString = connectionString;
	private readonly string _schema = schema;
	private readonly string _tableName = tableName;
	private readonly int _retainDays = retainDays;
	private readonly ILogger<SerilogCleanup> _logger = logger;

	public CancellationToken CancellationToken { get; set; }	

	public async Task Invoke()
	{
		_logger.LogDebug("Deleting logs older than {RetainDays} days", _retainDays);

		using var cn = new SqlConnection(_connectionString);
		int deleted = 0;		

		try
		{
			var sw = Stopwatch.StartNew();
			do
			{
				int chunk = await DeleteTopAsync(cn, _schema, _tableName, _retainDays, 10);
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
		var sql = 
			$@"DELETE TOP ({chunkSize}) 
			FROM [{schema}].[{tableName}]
			WHERE [Timestamp] < DATEADD(DAY, -{retainDays}, GETUTCDATE())";
			
		return await connection.ExecuteAsync(sql);
	}	
}
