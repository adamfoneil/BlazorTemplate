using Serilog.Context;

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
}
