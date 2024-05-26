using Application.Client;
using Domain;
using Domain.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Claims;

namespace Application.Components.Account
{
    // This is a server-side AuthenticationStateProvider that revalidates the security stamp for the connected user
    // every 30 minutes an interactive circuit is connected. It also uses PersistentComponentState to flow the
    // authentication state to the client which is then fixed for the lifetime of the WebAssembly application.
    internal sealed class PersistingRevalidatingAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly PersistentComponentState _state;
        private readonly IdentityOptions _options;
        private readonly PersistingComponentStateSubscription _subscription;

        private Task<AuthenticationState>? authenticationStateTask;

        public PersistingRevalidatingAuthenticationStateProvider(
            ILoggerFactory loggerFactory,
            IServiceScopeFactory serviceScopeFactory,
            PersistentComponentState persistentComponentState,
            IOptions<IdentityOptions> optionsAccessor)
            : base(loggerFactory)
        {
            _scopeFactory = serviceScopeFactory;
            _state = persistentComponentState;
            _options = optionsAccessor.Value;

            AuthenticationStateChanged += OnAuthenticationStateChanged;
            _subscription = _state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
        }

        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

        protected override async Task<bool> ValidateAuthenticationStateAsync(
            AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            // Get the user manager from a new scope to ensure it fetches fresh data
            await using var scope = _scopeFactory.CreateAsyncScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            return await ValidateSecurityStampAsync(userManager, authenticationState.User);
        }

        private async Task<bool> ValidateSecurityStampAsync(UserManager<ApplicationUser> userManager, ClaimsPrincipal principal)
        {
            var user = await userManager.GetUserAsync(principal);
            if (user is null)
            {
                return false;
            }
            else if (!userManager.SupportsUserSecurityStamp)
            {
                return true;
            }
            else
            {
                var principalStamp = principal.FindFirstValue(_options.ClaimsIdentity.SecurityStampClaimType);
                var userStamp = await userManager.GetSecurityStampAsync(user);
                return principalStamp == userStamp;
            }
        }

        private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
        {
            authenticationStateTask = task;
        }

        private async Task OnPersistingAsync()
        {
            if (authenticationStateTask is null)
            {
                throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");
            }

            var authenticationState = await authenticationStateTask;
            var principal = authenticationState.User;

            if (principal.Identity?.IsAuthenticated ?? false)
            {
                var userInfo = principal.FromClaims<UserInfo>();
                _state.PersistAsJson(nameof(UserInfo), userInfo);
            }
        }

        protected override void Dispose(bool disposing)
        {
            _subscription.Dispose();
            AuthenticationStateChanged -= OnAuthenticationStateChanged;
            base.Dispose(disposing);
        }
    }
}
