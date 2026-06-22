using LoginService.Application.Common;
using LoginService.EndPoints;

namespace LoginService.Application.Login
{
    public interface ILoginService
    {
        Task<Result<string>> GetJWTToken(UserLogin userLogin, CancellationToken cancellationToken = default);
    }
}
