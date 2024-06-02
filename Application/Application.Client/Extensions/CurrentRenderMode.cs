namespace Application.Client.Extensions;

public enum RenderLocationOptions
{
	Server,
	Client
}

public static class CurrentRenderLocation
{
	/// <summary>
	/// adapted from https://stackoverflow.com/a/77703444/25057433
	/// </summary>
	public static RenderLocationOptions Value => OperatingSystem.IsBrowser() ? RenderLocationOptions.Client : RenderLocationOptions.Server;
}
