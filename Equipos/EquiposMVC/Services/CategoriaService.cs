using System.Net;
using System.Net.Http.Json;
using EquiposMVC.Models;

namespace EquiposMVC.Services;

public class CategoriaService
{
    private readonly HttpClient _http;

    public CategoriaService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<CategoriaViewModel>> ListarAsync()
    {
        var r = await _http.GetAsync("categorias");
        ApiHelper.Verificar(r);
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<List<CategoriaViewModel>>() ?? new();
    }

    public async Task<CategoriaViewModel?> ObtenerAsync(int id)
    {
        var r = await _http.GetAsync($"categorias/{id}");
        if (r.StatusCode == HttpStatusCode.NotFound) return null;
        ApiHelper.Verificar(r);
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<CategoriaViewModel>();
    }

    public async Task<(bool Ok, string? Error)> CrearAsync(CategoriaViewModel m) =>
        await ApiHelper.Resultado(await _http.PostAsJsonAsync("categorias", m));

    public async Task<(bool Ok, string? Error)> ActualizarAsync(CategoriaViewModel m) =>
        await ApiHelper.Resultado(await _http.PutAsJsonAsync($"categorias/{m.Id}", m));

    public async Task<(bool Ok, string? Error)> EliminarAsync(int id) =>
        await ApiHelper.Resultado(await _http.DeleteAsync($"categorias/{id}"));
}