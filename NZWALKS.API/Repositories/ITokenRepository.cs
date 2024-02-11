using Microsoft.AspNetCore.Identity;

namespace NZWALKS.API.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
