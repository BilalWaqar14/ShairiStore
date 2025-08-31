namespace ShairiStore.Services;

public interface ITokenService
{
    Task<string> CreateTokenAsync(ApplicationUser user, IList<string> roles);
}
