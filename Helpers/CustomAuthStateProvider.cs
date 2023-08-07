using FightArchive.Data.Entities;
using FightArchive.Services;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace FightArchive.Helpers;

public class FightArchiveAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly AccountService _accountService;

    public Account CurrentUser { get; private set; } = new();

    public FightArchiveAuthenticationStateProvider(AccountService accountService)
    {
        _accountService = accountService;
        AuthenticationStateChanged += OnAuthenticationStateChangedAsync;
    }

    public async Task LoginAsync(string email, string password)
    {
        ClaimsPrincipal principal = new();
        Account? account = await _accountService.GetByEmailAsync(email);


        if (account is not null && Argon2.Verify(account.PasswordHash, password))
        {
            CurrentUser = account;
            principal = account.ToClaimsPrincipal(); 
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public async Task LogoutAsync()
    {
        await _accountService.ClearBrowserUserDataAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new())));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsPrincipal principal = new();
        Account? account = await _accountService.FetchUserFromBrowserAsync();
        if (account is not null)
        {
            Account? authenticatedUser =
                await _accountService.GetByEmailAsync(account.Email);

            if (authenticatedUser is not null)
            {
                CurrentUser = authenticatedUser;
                principal = authenticatedUser.ToClaimsPrincipal();
            }
        }

        return new(principal);
    }

    public void Dispose() => AuthenticationStateChanged -= OnAuthenticationStateChangedAsync;

    private async void OnAuthenticationStateChangedAsync(Task<AuthenticationState> task)
    {
        var authenticationState = await task;

        if (authenticationState is not null)
            CurrentUser = Account.FromClaimsPrincipal(authenticationState.User);
    }
}
