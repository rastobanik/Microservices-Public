using LoginService.Application.Common;
using LoginService.EndPoints;

namespace LoginService.Application.Login
{
    public class LoginService : ILoginService
    {
        public async Task<Result<string>> GetJWTToken(UserLogin userLogin, CancellationToken cancellationToken = default)
        {
            return new Result<string>() { Success = true, Value = "aaabbbbb" }; 
        }
    }
}
