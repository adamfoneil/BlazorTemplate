using Dapper.QX;
using Dapper.QX.Attributes;

namespace Application.Components.Pages.Serilog;

public class LogEntryResult
{
	public long Id { get; set; }
	public string Message { get; set; } = default!;	
	public string Level { get; set; } = default!;
	public DateTime? TimeStamp { get; set; }
	public string? Exception { get; set; }
	public string? Properties { get; set; }
	public string? UserName { get; set; }
	public string? SourceContext { get; set; } = default!;
	public string? RequestId { get; set; } = default!;
	public double? Elapsed { get; set; }
	public string? RequestPath { get; set; }
	public string? CommandText { get; set; }
}

internal class LogQuery(string schema, string tableName) : Query<LogEntryResult>(
	$@"SELECT * FROM [{schema}].[{tableName}] WHERE {{where}} ORDER BY [Timestamp] DESC {{offset}}")
{
	[Offset(30)]
	public int? Page { get; set; } = 0;

	[Where("[Level]=@level")]
	public string? Level { get; set; }

	[Where("[Message] LIKE CONCAT('%', @message, '%')")]
	public string? MessageLike { get; set; }

	[Where("[Exception] LIKE CONCAT('%', @exception, '%'")]
	public string? ExceptionLike { get; set; }

	[Where("[UserName]=@userName")]
	public string? UserName { get; set; }

	[Where("[RequestId]=@requestId")]
	public string? RequestId { get; set; }

	[Where("[SourceContext] LIKE CONCAT('%', @sourceContext, '%')")]
	public string? SourceContextLike { get; set; }
}
