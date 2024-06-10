using Dapper.QX;
using Dapper.QX.Attributes;

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
	WHERE 
		[RequestPath] NOT LIKE '/_blazor%' AND [RequestPath] NOT LIKE '/_framework%'
	GROUP BY 
		[SourceContext], [Level], [RequestPath], [UserName]")
{
	[Where("[RequestPath] NOT IN @ExcludeRequestPath")]
	public string[]? ExcludeRequestPath { get; set; }
}
