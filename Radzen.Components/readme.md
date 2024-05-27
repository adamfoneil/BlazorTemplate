This project has components that wrap several [Radzen](https://blazor.radzen.com/) components in ways that I use in my Blazor apps.

- [GridControls](https://github.com/adamfoneil/LiteInvoice3/blob/master/Radzen.Components/GridControls.razor) has typical save/cancel/edit/delete buttons for grids, along with "are you sure?" [dialog prompt](https://github.com/adamfoneil/LiteInvoice3/blob/master/Radzen.Components/GridControls.razor#L47) when deleting.
- [ActiveFilter](https://github.com/adamfoneil/LiteInvoice3/blob/master/Radzen.Components/ActiveFilter.razor) is a dropdown with "Active" and "Inactive" values that I use to filter grids.
- [GridInsertButton](https://github.com/adamfoneil/LiteInvoice3/blob/master/Radzen.Components/GridInsertButton.razor) is a button I use to add rows to grids.
- [IconNavLink](https://github.com/adamfoneil/LiteInvoice3/blob/master/Radzen.Components/IconNavLink.razor) combines the built-in `NavLink` with `RadzenIcon`, which uses [material icons](https://fonts.google.com/icons)
- [YesNoSwitch](https://github.com/adamfoneil/LiteInvoice3/blob/master/Radzen.Components/YesNoSwitch.razor) combines a [RadzenSwitch](https://blazor.radzen.com/switch) with adjacent Yes/No text
- The abstract [GridHelper](https://github.com/adamfoneil/LiteInvoice3/blob/master/Radzen.Components/Abstract/GridHelper.cs) class is used to encapsuate CRUD actions on grids to simplify the resulting markup.
  - Example [ProjectsGridHelper.cs](https://github.com/adamfoneil/LiteInvoice3/blob/master/WebApp/WebApp/Components/Pages/Setup/Projects.GridHelper.cs)
  - in use on [Projects.razor](https://github.com/adamfoneil/LiteInvoice3/blob/master/WebApp/WebApp/Components/Pages/Setup/Projects.razor)

I offer this as a NuGet package **AO.Radzen.Components** available here: https://aosoftware.blob.core.windows.net/packages/index.json
