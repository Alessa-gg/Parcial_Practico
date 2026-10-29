using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EquiposReportes.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SinSesionAttribute : Attribute { }

/// <summary>Filtro global: solo entra quien inició sesión con rol Admin.</summary>
public class SoloAdminFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.OfType<SinSesionAttribute>().Any()) return;

        var session = context.HttpContext.Session;
        if (string.IsNullOrEmpty(session.GetString("JWToken")) || session.GetString("Rol") != "Admin")
            context.Result = new RedirectToActionResult("Login", "Auth", null);
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
