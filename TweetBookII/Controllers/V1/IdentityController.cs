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
        if (!ModelState.IsValid)
        {
            return BadRequest(new AuthFailedResponse 
            { 
                Errors = ModelState.Values.SelectMany(_ => _.Errors.Select(_ => _.ErrorMessage))
            });
        }
        var authResponse = await _identityService.RegisterUserAsync(userRegistrationRequest.Email!, userRegistrationRequest.Password!);
        if (!authResponse.Succeded)
        {
            return BadRequest(new AuthFailedResponse
            {
                Errors = authResponse.Errors
            });
        }
        return Ok(new AuthSuccessResponse
        {
            Token = authResponse.Token!,
            RefreshToken = authResponse.RefreshToken!
        });
    }

    [HttpPost(ApiRoutes.Identity.Login)]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest userLoginRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new AuthFailedResponse
            {
                Errors = ModelState.Values.SelectMany(_ => _.Errors.Select(_ => _.ErrorMessage))
            });
        }
        
        var authResponse = await _identityService.LoginAsync(userLoginRequest.Email, userLoginRequest.Password);
        
        if (!authResponse.Succeded)
        {
            return BadRequest(new AuthFailedResponse
            {
                Errors = authResponse.Errors
            });
        }
        
        return Ok(new AuthSuccessResponse
        {
            Token = authResponse.Token!,
            RefreshToken = authResponse.RefreshToken!
        });
    }

    [HttpPost(ApiRoutes.Identity.Refresh)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        var authResponse = await _identityService.RefreshTokenAsync(refreshTokenRequest.Token, refreshTokenRequest.RefreshToken);
        if (authResponse.Succeded)
        {
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token!,
                RefreshToken = authResponse.RefreshToken!
            });
        }
        return BadRequest(new AuthFailedResponse
        {
            Errors = authResponse.Errors
        });
    }
}
