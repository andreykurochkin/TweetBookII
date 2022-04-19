using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetBookII.Domain;
public class AuthorizationResult
{
    public bool Succeded { get; set; }
    public string? Token { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}
