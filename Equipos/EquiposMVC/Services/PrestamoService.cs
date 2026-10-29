using System.Net.Http.Json;
using EquiposMVC.Models;

namespace EquiposMVC.Services;

public class PrestamoService
{
    private readonly HttpClient _http;

    public PrestamoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<PrestamoViewModel>> ListarAsync(bool soloVencidos)
    {
        var r = await _http.GetAsync("prestamos" + (soloVencidos ? "?soloVencidos=true" : ""));
        ApiHelper.Verificar(r);
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<List<PrestamoViewModel>>() ?? new();
    }

    public async Task<List<UsuarioViewModel>> ListarUsuariosAsync()
    {
        var r = await _http.GetAsync("usuarios");
        ApiHelper.Verificar(r);
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<List<UsuarioViewModel>>() ?? new();
    }

    public async Task<(bool Ok, string? Error)> RegistrarAsync(PrestamoCreateViewModel m) =>
        await ApiHelper.Resultado(await _http.PostAsJsonAsync("prestamos", m));

    public async Task<(bool Ok, string? Error)> DevolverAsync(int id) =>
        await ApiHelper.Resultado(await _http.PutAsync($"prestamos/{id}/devolucion", null));
}
