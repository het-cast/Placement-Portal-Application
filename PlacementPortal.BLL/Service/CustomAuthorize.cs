namespace PlacementPortal.BLL.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class CustomAuthorize : Attribute, IAuthorizationFilter
{
  private readonly string[] _roles;

  private readonly Dictionary<string, string> _redirectUrls;

  public CustomAuthorize(string[] roles, params string[] redirectUrls)
  {
    _roles = roles;

    _redirectUrls = new Dictionary<string, string>();

    // Here mapping each role to the given redirect url
    for (int i = 0; i < roles.Length && i < redirectUrls.Length; i++)
    {
      _redirectUrls[roles[i]] = redirectUrls[i];
    }
  }

  public void OnAuthorization(AuthorizationFilterContext context)
  {
    var token = context.HttpContext.Request.Cookies["AuthToken"];

    if (string.IsNullOrEmpty(token))
    {
      context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
      {
        controller = "Authentication",
        action = "Logout"
      }));
      return;
    }

    var handler = new JwtSecurityTokenHandler();
    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

    if (jwtToken == null)
    {
      context.Result = new UnauthorizedResult();
      return;
    }

    var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

    if (roleClaim == null || !_roles.Contains(roleClaim))
    {
      context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
      {
        controller = "Authentication",
        action = "Logout"
      }));
      return;
    }

    if (_redirectUrls.TryGetValue(roleClaim, out var redirectUrl))
    {
      if (!string.IsNullOrEmpty(redirectUrl))
      {
        var currentPath = context.HttpContext.Request.Path.Value?.TrimEnd('/');

        if (!string.Equals(currentPath, redirectUrl, StringComparison.OrdinalIgnoreCase))
        {
          context.Result = new RedirectResult(redirectUrl);
          return;
        }
      }
    }
  }
}

