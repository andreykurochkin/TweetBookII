using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetBookII.Domain;
public class AuthenticationResult
{
    public bool Succeded { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}
