using LoginService.EndPoints;

namespace LoginService.Application.Login
{
    public class LoginService : ILoginService
    {
        public async Task<string> GetJWTToken(UserLogin userLogin, CancellationToken cancellationToken = default)
        {
            return string.Empty;
        }
    }
}
