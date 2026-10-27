using System.Net;

namespace EquiposMVC.Services;

internal static class ApiHelper
{
    public static void Verificar(HttpResponseMessage r)
    {
        if (r.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden || (int)r.StatusCode >= 500)
            throw new ApiException(r.StatusCode);
    }

    public static async Task<(bool Ok, string? Error)> Resultado(HttpResponseMessage r)
    {
        Verificar(r);
        if (r.IsSuccessStatusCode) return (true, null);

        var msg = (await r.Content.ReadAsStringAsync()).Trim().Trim('"');
        return (false, string.IsNullOrWhiteSpace(msg) ? "No se pudo completar la operación." : msg);
    }
}