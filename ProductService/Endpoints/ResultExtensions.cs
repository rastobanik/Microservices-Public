using ProductService.Application.Common;

namespace ProductService.Endpoints
{
    public static class ResultExtensions
    {
        public static IResult ToHttp<T>(this Result<T>? result)
        => result is null
           ? Results.StatusCode(StatusCodes.Status500InternalServerError)
           : result.Success
               ? Results.Ok(result.Value)
               : Results.NotFound(result.Error);
    }
}
