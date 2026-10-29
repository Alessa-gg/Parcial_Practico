using System.Net;
using System.Net.Http.Json;
using EquiposMVC.Models;

namespace EquiposMVC.Services;

public class AuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<(LoginResponse? Data, string? Error)> LoginAsync(LoginViewModel m)
    {
        var r = await _http.PostAsJsonAsync("auth/login", new { m.Username, m.Password });

        // Un 401 aquí significa credenciales incorrectas, no sesión expirada.
        if (r.StatusCode == HttpStatusCode.Unauthorized)
            return (null, "Usuario o contraseña incorrectos.");
        if ((int)r.StatusCode >= 500)
            throw new ApiException(r.StatusCode);
        if (!r.IsSuccessStatusCode)
            return (null, "No se pudo iniciar sesión.");

        return (await r.Content.ReadFromJsonAsync<LoginResponse>(), null);
    }

    public async Task<(bool Ok, string? Error)> RegistrarAsync(RegistrarUsuarioViewModel m) =>
        await ApiHelper.Resultado(await _http.PostAsJsonAsync("auth/register", m));
}
