using Serilog;
using Serilog.Configuration;
using Serilog.Context;
using Serilog.Sinks.MSSqlServer;
using System.Data;

namespace Application.Extensions;

internal static class SerilogExtensions
{
	internal static void UseSerilogUserName(this WebApplication app)
	{
		app.Use(async (context, next) =>
		{
			using (LogContext.PushProperty("UserName", context.User.Identity?.Name ?? "Anonymous"))
			{
				await next.Invoke();
			}
		});
	}

	internal static void SqlServerCustomConfig(this LoggerSinkConfiguration config, string connectionString)
	{
		var columnOptions = new ColumnOptions()
		{
			AdditionalColumns =
			[
				new("UserName", SqlDbType.NVarChar, true, 50),
				new("SourceContext", SqlDbType.NVarChar, true, 100),
				new("RequestId", SqlDbType.NVarChar, true, 100),
				new("Elapsed", SqlDbType.Float, true),
				new("RequestPath", SqlDbType.NVarChar, true, 256),
				new("CommandText", SqlDbType.NVarChar, true),
			]
		};
		columnOptions.Id.DataType = SqlDbType.BigInt;

		config.MSSqlServer(connectionString, new MSSqlServerSinkOptions()
		{
			SchemaName = "log",
			TableName = "Serilog",
			AutoCreateSqlTable = true,
		}, columnOptions: columnOptions);
	}
}
