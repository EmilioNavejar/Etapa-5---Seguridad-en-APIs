using System.Security.Claims;

namespace BankAPI.Services
{
    public interface IAuthenticationService
    {
        int GetCurrentClientId(ClaimsPrincipal user);
    }

    public class AuthenticationService : IAuthenticationService
{
    public int GetCurrentClientId(ClaimsPrincipal user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        var clientIdClaim = user.FindFirst("ClientId")?.Value;
        if (clientIdClaim == null) throw new InvalidOperationException(" 'ClientId' No fue encontrado.");

        if (!int.TryParse(clientIdClaim, out int clientId))
        {
            throw new InvalidOperationException("'ClientId' Invalido");
        }

        return clientId;
    }
}

}


