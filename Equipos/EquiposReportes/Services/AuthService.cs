using System.Net;
using System.Net.Http.Json;
using EquiposReportes.ViewModels;

namespace EquiposReportes.Services;

// El acceso a los reportes se valida contra /api/auth/login de EquiposAPI.
public class AuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<(LoginResponse? Data, string? Error)> LoginAsync(LoginViewModel m)
    {
        HttpResponseMessage r;
        try
        {
            r = await _http.PostAsJsonAsync("auth/login", new { m.Username, m.Password });
        }
        catch (HttpRequestException)
        {
            return (null, "No se pudo conectar con la API. Verifica que EquiposAPI esté en ejecución.");
        }

        if (r.StatusCode == HttpStatusCode.Unauthorized)
            return (null, "Usuario o contraseña incorrectos.");
        if (!r.IsSuccessStatusCode)
            return (null, "No se pudo iniciar sesión.");

        return (await r.Content.ReadFromJsonAsync<LoginResponse>(), null);
    }
}
