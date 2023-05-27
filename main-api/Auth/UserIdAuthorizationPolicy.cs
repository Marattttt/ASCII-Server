using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace api.Policies;
public static class UserIdAuthorizationPolicy
{
    public static AuthorizationPolicy UserIdPolicy()
    {
        return new AuthorizationPolicyBuilder()
            .RequireClaim("UserId")
            .Build();
    }

    // Define more policies as needed
}
