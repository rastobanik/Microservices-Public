using LoginService.EndPoints;

namespace LoginService.Application.Login
{
    public interface ILoginService
    {
        Task<string> GetJWTToken(UserLogin userLogin, CancellationToken cancellationToken = default);
    }
}
