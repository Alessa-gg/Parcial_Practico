using EquiposMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace EquiposMVC.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    private readonly IModelMetadataProvider _metadata;

    public ApiExceptionFilter(IModelMetadataProvider metadata)
    {
        _metadata = metadata;
    }

    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case ApiException { Status: System.Net.HttpStatusCode.Unauthorized }:
                context.HttpContext.Session.Clear();
                context.Result = new RedirectToActionResult("Login", "Auth", new { expirada = true });
                break;

            case ApiException { Status: System.Net.HttpStatusCode.Forbidden }:
                context.Result = Vista(context, "AccesoDenegado", 403);
                break;

            case ApiException:
            case HttpRequestException:
                context.Result = Vista(context, "ErrorGenerico", 500);
                break;

            default:
                return;
        }

        context.ExceptionHandled = true;
    }

    private ViewResult Vista(ExceptionContext context, string nombre, int status) => new()
    {
        ViewName = nombre,
        StatusCode = status,
        ViewData = new ViewDataDictionary(_metadata, context.ModelState)
    };
}
