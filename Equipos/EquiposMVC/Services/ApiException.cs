using System.Net;

namespace EquiposMVC.Services;

public class ApiException : Exception
{
    public HttpStatusCode Status { get; }

    public ApiException(HttpStatusCode status) : base($"La API respondió {(int)status}.")
    {
        Status = status;
    }
}