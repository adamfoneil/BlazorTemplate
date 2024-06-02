using Radzen;

namespace AO.Radzen.Components.Abstract;

public abstract class GridHelper<TData>(DialogService dialogService)
{
    private readonly DialogService _dialog = dialogService;

    public IEnumerable<TData> Data { get; set; } = [];
    public abstract Task<IEnumerable<TData>> QueryAsync();

    public async Task RefreshAsync()
    {
        Data = await QueryAsync();
        await OnRefreshAsync();
        OnRefresh?.Invoke(this, new EventArgs());
    }

    public async Task SaveRowAsync(TData row)
    {
        try
        {
            await OnSaveAsync(row);
            await RefreshAsync();
            OnSuccess?.Invoke(this, new EventArgs());
        }
        catch (Exception exc)
        {
            OnError?.Invoke(this, exc);
            await _dialog.Alert(exc.Message);
        }
    }

    public async Task DeleteRowAsync(TData row)
    {
        try
        {
            await OnDeleteAsync(row);
            await RefreshAsync();
            OnSuccess?.Invoke(this, new EventArgs());
        }
        catch (Exception exc)
        {
            OnError?.Invoke(this, exc);
            await _dialog.Alert(exc.Message);
        }
    }

    public abstract Task OnSaveAsync(TData data);
    public abstract Task OnDeleteAsync(TData data);

    public EventHandler<Exception>? OnError { get; set; }
    public EventHandler? OnSuccess { get; set; }
    public EventHandler? OnRefresh { get; set; }

    protected virtual async Task OnRefreshAsync() { await Task.CompletedTask; }
}
