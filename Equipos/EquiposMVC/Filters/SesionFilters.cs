using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace EquiposMVC.Filters;

/// <summary>Marca una acción como accesible sin haber iniciado sesión (el login).</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SinSesionAttribute : Attribute { }

/// <summary>Filtro global: sin token en Session se redirige al login.</summary>
public class SesionRequeridaFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var libre = context.ActionDescriptor.EndpointMetadata.OfType<SinSesionAttribute>().Any();
        if (libre) return;

        if (string.IsNullOrEmpty(context.HttpContext.Session.GetString("JWToken")))
            context.Result = new RedirectToActionResult("Login", "Auth", null);
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}

/// <summary>Limita una acción a ciertos roles según lo guardado en Session (la API vuelve a validar).</summary>
public class SoloRolesAttribute : ActionFilterAttribute
{
    private readonly string[] _roles;

    public SoloRolesAttribute(params string[] roles)
    {
        _roles = roles;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var rol = context.HttpContext.Session.GetString("Rol");
        if (rol is null || !_roles.Contains(rol))
        {
            var services = context.HttpContext.RequestServices;
            context.Result = new ViewResult
            {
                ViewName = "AccesoDenegado",
                StatusCode = 403,
                ViewData = new ViewDataDictionary(
                    services.GetRequiredService<IModelMetadataProvider>(), context.ModelState),
                TempData = services.GetRequiredService<ITempDataDictionaryFactory>().GetTempData(context.HttpContext)
            };
        }
    }
}
