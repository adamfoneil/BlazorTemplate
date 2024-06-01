namespace Application.Client;

/// <summary>
/// provides a way to trigger UI feedback when the API is working
/// </summary>
public class ApiEventHandler
{
	public bool IsWorking { get; private set; }
	public bool ErrorOccurred { get; private set; }	
	public string? ErrorMessage { get; private set; }

	/// <summary>
	/// user can toggle this, so this has a public setter
	/// </summary>
    public bool ShowErrorMessage { get; set; }

    public event Action? OnStarted;
	public event Action? OnStopped;
	public event Action<string>? OnError;

	public void Start()
	{
		ErrorOccurred = false;
		IsWorking = true;
		OnStarted?.Invoke();
	}

	public void Stop()
	{
		IsWorking = false;
		OnStopped?.Invoke();
	}

	public void Error(string message)
	{
		ShowErrorMessage = true;
		ErrorOccurred = true;
		ErrorMessage = message;
		OnError?.Invoke(message);
	}
}
