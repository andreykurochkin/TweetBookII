﻿namespace TweetBookII.Contracts.V1.Requests;
public class RefreshTokenRequest
{
    public string? Token { get; set; } = null!;
    public string? RefreshToken { get; set; } = null!;
}
