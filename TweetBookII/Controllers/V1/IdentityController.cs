using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TweetBookII.Contracts.V1;
using TweetBookII.Contracts.V1.Requests;
using TweetBookII.Contracts.V1.Responses;
using TweetBookII.Infrastructure.Services.Base;

namespace TweetBookII.Controllers.V1;

public class IdentityController : Controller
{
    private readonly IIdentityService _identityService;

    public IdentityController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost(ApiRoutes.Identity.Registration)]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequest userRegistrationRequest)
    {
        var userRegistrationResult = await _identityService.RegisterUserAsync(userRegistrationRequest.Email!, userRegistrationRequest.Password!);
        if (!userRegistrationResult.Succeded)
        {
            return BadRequest(new AuthFailedResponse
            {
                Errors = userRegistrationResult.Errors
            });
        }
        return Ok(new AuthSuccessResponse
        {
            Token = userRegistrationResult.Token!,
        });
    }
}
