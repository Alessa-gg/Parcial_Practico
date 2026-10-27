using System.Net;
using System.Net.Http.Json;
using EquiposMVC.Models;

namespace EquiposMVC.Services;

public class EquipoService
{
    private readonly HttpClient _http;

    public EquipoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<EquipoViewModel>> ListarAsync(string? texto, int? categoriaId, string? estado)
    {
        var qs = new List<string>();
        if (!string.IsNullOrWhiteSpace(texto)) qs.Add("texto=" + Uri.EscapeDataString(texto));
        if (categoriaId.HasValue) qs.Add("categoriaId=" + categoriaId.Value);
        if (!string.IsNullOrWhiteSpace(estado)) qs.Add("estado=" + Uri.EscapeDataString(estado));

        var url = "equipos" + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");
        var r = await _http.GetAsync(url);
        ApiHelper.Verificar(r);
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<List<EquipoViewModel>>() ?? new();
    }

    public async Task<EquipoViewModel?> ObtenerAsync(int id)
    {
        var r = await _http.GetAsync($"equipos/{id}");
        if (r.StatusCode == HttpStatusCode.NotFound) return null;
        ApiHelper.Verificar(r);
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<EquipoViewModel>();
    }

    public async Task<(bool Ok, string? Error)> CrearAsync(EquipoFormViewModel m) =>
        await ApiHelper.Resultado(await _http.PostAsJsonAsync("equipos", m));

    public async Task<(bool Ok, string? Error)> ActualizarAsync(EquipoFormViewModel m) =>
        await ApiHelper.Resultado(await _http.PutAsJsonAsync($"equipos/{m.Id}", m));

    public async Task<(bool Ok, string? Error)> EliminarAsync(int id) =>
        await ApiHelper.Resultado(await _http.DeleteAsync($"equipos/{id}"));
}