namespace Application.Client;

/// <summary>
/// provides a way to trigger UI feedback when the backend is working
/// </summary>
public class BackendWorkIndicator
{
    public bool IsWorking { get; private set; }

    public event Action? OnStarted;
    public event Action? OnStopped;

    public void Start()
    {
        IsWorking = true;
        OnStarted?.Invoke();
    }

    public void Stop()
    {
        IsWorking = false;
        OnStopped?.Invoke();
    }
}
