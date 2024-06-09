namespace Application.Features.Serilog;

public class SerilogOptions
{
	public string Schema { get; set; } = "log";
	public string TableName { get; set; } = "Serilog";
	public int RetainDays { get; set; } = 5;
}
