namespace Application.Client.Extensions;

public enum RenderModeOptions
{
	Server,
	WebAssembly
}

public static class CurrentRenderMode
{
	/// <summary>
	/// https://stackoverflow.com/a/77703444/25057433
	/// </summary>
	public static RenderModeOptions Value => OperatingSystem.IsBrowser() ? RenderModeOptions.WebAssembly : RenderModeOptions.Server;
}
