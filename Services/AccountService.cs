using FightArchive.Data;
using FightArchive.Data.Entities;
using FightArchive.Data.Entities.DTO;
using FightArchive.Helpers;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FightArchive.Services;

public class AccountService
{
    private readonly AppSettings _appSettings;
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly DataContext _database;
    private readonly string storageKey = "FLFightArchives";

    public AccountService(IOptions<AppSettings> appSettings,
        DataContext database,
        ProtectedLocalStorage protectedLocalStorage)
    {
        _appSettings = appSettings.Value;
        _database = database;
        _protectedLocalStorage = protectedLocalStorage;
    }

    public async Task<Account?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));
        Account? account = await _getByEmailAsync(email);
        if (account is not null)
            await PersistAccountToStorageAsync(account);
        return account;
    }

    public async Task<Account?> GetByIdAsync(string accountId)
    {
        if (Guid.TryParse(accountId, out Guid id))
        {
            Account? account = await _getAsync(id);
            if (account is not null)
                await PersistAccountToStorageAsync(account);
            return account;
        }
        else
            throw new FormatException(nameof(accountId));
    }

    public string GenerateAccountToken(AccountStorage accountStorage)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        byte[] key = Encoding.ASCII.GetBytes(_appSettings.LogsFolder);
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(new[] 
            { 
                new Claim(ClaimTypes.Email, accountStorage.Email),
                new Claim(ClaimTypes.Expiration, accountStorage.Expires.ToString())
            }),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
        };
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task PersistAccountToStorageAsync(Account account)
    {
        string json = JsonConvert.SerializeObject(account);
        await _protectedLocalStorage.SetAsync(storageKey, json);
    }

    public async Task<Account?> FetchUserFromBrowserAsync()
    {
        // When Blazor Server is rendering at server side,
        // there is no local storage. Therefore, put an empty
        // try catch to avoid error.
        try
        {
            var fetchedAccountResult = 
                await _protectedLocalStorage.GetAsync<string>(storageKey);
            
            if (fetchedAccountResult.Success && !string.IsNullOrEmpty(fetchedAccountResult.Value))
            {
                Account? account = JsonConvert.DeserializeObject<Account>(fetchedAccountResult.Value);
                return account ?? throw new Exception("Failed to deserialize account!");
            }
        }
        catch
        {
        }

        return null;
    }

    public async Task ClearBrowserUserDataAsync() =>
        await _protectedLocalStorage.DeleteAsync(storageKey);

    private async Task<Account?> _getAsync(Guid id) =>
        await _database.Accounts!
        .Include(i => i.Role)
        .Include(i => i.Characters)
        .FirstOrDefaultAsync(f => f.Id == id);

    private async Task<Account?> _getByEmailAsync(string email) =>
        await _database.Accounts!
        .Include(i => i.Role)
        .Include(i => i.Characters)
        .FirstOrDefaultAsync(f => f.Email.Equals(email));
}
