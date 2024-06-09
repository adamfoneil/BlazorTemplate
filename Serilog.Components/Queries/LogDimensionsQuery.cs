using Dapper.QX;

namespace Serilog.Components.Queries;

public class LogDimensionsResult
{
	public string SourceContext { get; set; } = default!;
	public string Level { get; set; } = default!;
	public string? RequestPath { get; set; }
	public string? UserName { get; set; }
	public int Count { get; set; }
}

public class LogDimensionsQuery(string schema, string tableName) : Query<LogDimensionsResult>(
	@$"SELECT 
		[SourceContext], [Level], [RequestPath], [UserName], COUNT(1) AS [Count] 
	FROM 
		[{schema}].[{tableName}] 
	GROUP BY 
		[SourceContext], [Level], [RequestPath], [UserName]")
{
}
