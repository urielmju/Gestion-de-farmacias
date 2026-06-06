using System.Web.Mvc;
using System.Web.Routing;

namespace PIV_PF_ProyectoFinal.Filters
{
    /// <summary>
    /// Redirige al login si no hay sesion activa.
    /// Uso: [SessionAuthorize] sobre el controlador o el action.
    /// Uso con rol: [SessionAuthorize(Roles = "Administrador,Vendedor")]
    /// </summary>
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public string Roles { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;

            if (session["IdUsuario"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Account" },
                    { "action",     "Login"   }
                });
                return;
            }

            if (!string.IsNullOrEmpty(Roles))
            {
                string tipoActual = session["TipoUsuario"] as string ?? "";
                bool tieneAcceso = false;
                foreach (var rol in Roles.Split(','))
                {
                    if (rol.Trim() == tipoActual) { tieneAcceso = true; break; }
                }

                if (!tieneAcceso)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        { "controller", "Home"  },
                        { "action",     "Index" }
                    });
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
