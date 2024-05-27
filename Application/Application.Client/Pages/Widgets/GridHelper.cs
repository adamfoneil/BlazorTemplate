using AO.Radzen.Components.Abstract;
using Domain;
using Radzen;

namespace Application.Client.Pages.Widgets;

public class GridHelper(DialogService dialogService, ApiClient apiClient) : GridHelper<Widget>(dialogService)
{
	private readonly ApiClient Api = apiClient;

	public override async Task OnDeleteAsync(Widget data) => await Api.DeleteWidgetAsync(data.Id);

	public override async Task OnSaveAsync(Widget data) => await Api.SaveWidgetAsync(data);

	public override async Task<IEnumerable<Widget>> QueryAsync() => await Api.GetMyWidgetsAsync();
}
