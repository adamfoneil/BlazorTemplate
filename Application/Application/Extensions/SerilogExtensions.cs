using Serilog;
using Serilog.Configuration;
using Serilog.Context;
using Serilog.Sinks.MSSqlServer;
using Serilog.Templates;
using Serilog.Templates.Themes;
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

	internal const string Schema = "log";
	internal const string TableName = "Serilog";

	internal static ExpressionTemplate CustomConsoleOutput => new(
		"[{@t:HH:mm:ss} {SourceContext} <{UserName}> {@l:u3}{#if @tr is not null} ({substring(@tr,0,4)}:{substring(@sp,0,4)}){#end}] {@m}\n{@x}",
		theme: TemplateTheme.Literate);

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
			SchemaName = Schema,
			TableName = TableName,
			AutoCreateSqlTable = true,
		}, columnOptions: columnOptions);
	}

	internal static void AddSerilogCleanup(this IServiceCollection services, string connectionString, int retainDays) => 
		services.AddTransient(sp => new SerilogCleanup(connectionString, Schema, TableName, retainDays, sp.GetRequiredService<ILogger<SerilogCleanup>>()));
}
