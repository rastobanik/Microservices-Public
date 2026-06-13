using LoginService.Application.Login;
using Microsoft.AspNetCore.Mvc;

namespace LoginService.EndPoints
{
    public static class LoginEndpoint
    {
        public static void MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/product", GetLogin);
        }

        private static async Task<IResult> GetLogin(UserLogin login, [FromServices] ILoginService service, ILogger<Program> logger, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Endpoint called for users {UserName}", login.UserName);

            // volanie sluzby na najdenie objednavky s danym id
            var result = await service.GetJWTToken(login, cancellationToken);
            return result.ToHttp();
        }
    }
}
