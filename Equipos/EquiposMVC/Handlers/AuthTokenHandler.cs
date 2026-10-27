using System.Net.Http.Headers;

namespace EquiposMVC.Handlers;

public class AuthTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _accessor;

    public AuthTokenHandler(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _accessor.HttpContext?.Session.GetString("JWToken");
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return base.SendAsync(request, cancellationToken);
    }
}