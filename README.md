This brings together several opinions I have about how Blazor projects should be structured when supporting mixed WASM and Server components. It also adds several practical elements for real applications. Note also I'm using the new [.slnx](https://github.com/adamfoneil/BlazorTemplate/blob/master/BlazorTemplate.slnx) solution format.

- [Domain](https://github.com/adamfoneil/BlazorTemplate/tree/master/Domain) has entity classes
  - [Widget](https://github.com/adamfoneil/BlazorTemplate/blob/master/Domain/Widget.cs), a generic dummy table you can remove, but is there to demo a crud editing grid later
  - [ApplicationUser](https://github.com/adamfoneil/BlazorTemplate/blob/master/Domain/ApplicationUser.cs) has a `TimeZoneId` property, to demo a custom user property with some practical value
  - some low-level [extensions](https://github.com/adamfoneil/BlazorTemplate/blob/master/Domain/Extensions/IUserInfoExtensions.cs) for working with user info and Claims
- [Service](https://github.com/adamfoneil/BlazorTemplate/tree/master/Service) has business logic
  - the [ApplicationDbContext](https://github.com/adamfoneil/BlazorTemplate/blob/master/Service/ApplicationDbContext.cs)
  - a special claims factory [ApplicationUserClaimsPrincipalFactory](https://github.com/adamfoneil/BlazorTemplate/blob/master/Service/ApplicationUserClaimsPrincipalFactory.cs). This is what makes the user's time zone available to the client app.
- [Application](https://github.com/adamfoneil/BlazorTemplate/tree/master/Application) with its
  - frontend [Application.Client](https://github.com/adamfoneil/BlazorTemplate/tree/master/Application/Application.Client)
    - see [ApiClient](https://github.com/adamfoneil/BlazorTemplate/blob/master/Application/Application.Client/ApiClient.cs)
    - see [Widgets](https://github.com/adamfoneil/BlazorTemplate/tree/master/Application/Application.Client/Pages/Widgets) for a working Radzen grid example and related components
    - see [ApiEventHandler](https://github.com/adamfoneil/BlazorTemplate/blob/master/Application/Application.Client/ApiEventHandler.cs) to trigger UI feedback when the API is working, along with
    - [ApiEventUI](https://github.com/adamfoneil/BlazorTemplate/blob/master/Application/Application.Client/Components/ApiEventUI.razor) which uses a loading spinner and error message [Modal](https://github.com/adamfoneil/BlazorTemplate/blob/master/Application/Application.Client/Components/Modal.razor)
  - backend [Application](https://github.com/adamfoneil/BlazorTemplate/tree/master/Application/Application)
    - see [API endpoint setup](https://github.com/adamfoneil/BlazorTemplate/blob/master/Application/Application/Program.cs#L85-L87) and
    - [MapDbSet](https://github.com/adamfoneil/BlazorTemplate/blob/master/Application/Application/Extensions/DbContextExtensions.cs#L16) extension method
    - [time zone editing](https://github.com/adamfoneil/BlazorTemplate/blob/master/Application/Application/Components/Account/Pages/Manage/Index.razor#L32-L41) on the user profile manage page
- various Library projects are things that could be extracted to standalone packages, but are in the solution because they're still evolving:
  - [ApiClientBaseLibrary](https://github.com/adamfoneil/BlazorTemplate/tree/master/ApiClientBaseLibrary) for low-level http client operations
  - [AuthLibrary](https://github.com/adamfoneil/BlazorTemplate/tree/master/AuthLibrary) has WASM-specific stuff, such as
    - [CookieHandler.cs](https://github.com/adamfoneil/BlazorTemplate/blob/master/AuthLibrary/CookieHandler.cs)
    - a special [AddHttpClient](https://github.com/adamfoneil/BlazorTemplate/blob/master/AuthLibrary/ServiceCollectionExtensions.cs#L7) extension method that lets you setup an http client for WASM use, invoked
      - in the backend [here](https://github.com/adamfoneil/BlazorTemplate/blob/master/Application/Application/Program.cs#L52)
      - in the frontend [here](https://github.com/adamfoneil/BlazorTemplate/blob/master/Application/Application.Client/Program.cs#L18)
  - [Radzen.Components](https://github.com/adamfoneil/BlazorTemplate/tree/master/Radzen.Components) builds upon existing [Radzen](https://blazor.radzen.com/) components, notably
    - [GridControls.razor](https://github.com/adamfoneil/BlazorTemplate/blob/master/Radzen.Components/GridControls.razor) has standard crud controls
    - [GridHelper.cs](https://github.com/adamfoneil/BlazorTemplate/blob/master/Radzen.Components/Abstract/GridHelper.cs) abstract class eliminates a lot of grid code boilerplate    

# Help From
- [Dave Archer](https://github.com/ripteqdavid/sample-blazor8-auth-ms) on some Blazor WASM auth stuff
- [Meziantou](https://www.meziantou.net/modal-component-in-blazor.htm) on Blazor modals
- [Jon Hilton's community](https://community.practicaldotnet.io/home) for general support
