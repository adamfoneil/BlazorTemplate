namespace Serilog.Components;

public class SerilogOptions
{
	public string Schema { get; set; } = "log";
	public string TableName { get; set; } = "Serilog";
	public int RetainDays { get; set; } = 5;
	/// <summary>
	/// when deleting old logs, how many to delete at a time.
	/// Keep this to a low-ish number to avoid locking.
	/// </summary>
	public int DeleteChunkSize { get; set; } = 10;
}
