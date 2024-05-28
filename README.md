This brings together several opinions I have about how Blazor projects should be structured when supporting mixed WASM and Server components.

- [Domain](https://github.com/adamfoneil/BlazorTemplate/tree/master/Domain) has entity classes
  - [Widget](https://github.com/adamfoneil/BlazorTemplate/blob/master/Domain/Widget.cs), a generic dummy table you can remove, but is there to demo a crud editing grid later
  - [ApplicationUser](https://github.com/adamfoneil/BlazorTemplate/blob/master/Domain/ApplicationUser.cs) has a `TimeZoneId` property, to demo a custom user property with some practical value
  - some low-level [extensions](https://github.com/adamfoneil/BlazorTemplate/blob/master/Domain/Extensions/IUserInfoExtensions.cs) for working with user info and Claims
- [Service](https://github.com/adamfoneil/BlazorTemplate/tree/master/Service) has business logic
  - the [ApplicationDbContext](https://github.com/adamfoneil/BlazorTemplate/blob/master/Service/ApplicationDbContext.cs)
  - a special claims factory [ApplicationUserClaimsPrincipalFactory](https://github.com/adamfoneil/BlazorTemplate/blob/master/Service/ApplicationUserClaimsPrincipalFactory.cs)
- [Application](https://github.com/adamfoneil/BlazorTemplate/tree/master/Application) with its
  - frontend [Application.Client](https://github.com/adamfoneil/BlazorTemplate/tree/master/Application/Application.Client)
  - backend [Application](https://github.com/adamfoneil/BlazorTemplate/tree/master/Application/Application)
- various Library projects are things that could be extracted to standalone packages, but are in the solution because they're still evolving:
  - [ApiClientBaseLibrary](https://github.com/adamfoneil/BlazorTemplate/tree/master/ApiClientBaseLibrary)
  - [AuthLibrary](https://github.com/adamfoneil/BlazorTemplate/tree/master/AuthLibrary) has WASM-specific stuff
  - [Radzen.Components](https://github.com/adamfoneil/BlazorTemplate/tree/master/Radzen.Components) builds upon existing [Radzen](https://blazor.radzen.com/) components
