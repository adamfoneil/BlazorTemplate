using Domain;

namespace Application.Client;

public partial class ApiClient
{
	internal async Task DeleteWidgetAsync(int id) =>
		await DeleteAsync($"api/widgets/{id}");

	internal async Task<IEnumerable<Widget>> GetMyWidgetsAsync() =>
		await GetAsync<IEnumerable<Widget>>("api/widgets") ?? [];

	internal async Task<Widget> SaveWidgetAsync(Widget data) =>
		await PostWithInputAndResultAsync("api/widgets", data) ?? throw new Exception("widget not saved");
}
