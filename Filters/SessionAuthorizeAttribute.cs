using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class SessionAuthorizeAttribute : ActionFilterAttribute
{
    private readonly string _requiredRole;

    public SessionAuthorizeAttribute(string requiredRole = "")
    {
        _requiredRole = requiredRole.ToLower();
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        var tipoUsuario = session.GetString("tipo_usuario");
        var rol = session.GetString("Rol")?.ToLower();

        if (string.IsNullOrEmpty(tipoUsuario) || string.IsNullOrEmpty(rol))
        {
            context.Result = new RedirectToActionResult("Index", "Login", null);
            return;
        }

        if (!string.IsNullOrEmpty(_requiredRole) && rol != _requiredRole)
        {
            context.Result = new RedirectToActionResult("Index", "Login", null);
        }
    }
}
