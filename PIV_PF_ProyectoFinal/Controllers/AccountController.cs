using System.Linq;
using System.Web.Mvc;
using PIV_PF_ProyectoFinal.Models;
using PIV_PF_ProyectoFinal.ViewModels;

namespace PIV_PF_ProyectoFinal.Controllers
{
    public class AccountController : Controller
    {
        private PIV_PF_ProyectoFinalEntities1 db = new PIV_PF_ProyectoFinalEntities1();

        public AccountController()
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        public ActionResult Login()
        {
            if (Session["IdUsuario"] != null)
                return RedirectToAction("Index", "Home");

            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            string credencial = vm.Identificacion.Trim();

            var usuario = db.Usuarios
                            .FirstOrDefault(u =>
                                (u.Identificacion == credencial || u.Correo == credencial)
                                && u.Contrasena == vm.Contrasena
                                && u.Estado == "Activo");

            if (usuario == null)
            {
                ModelState.AddModelError("",
                    "Credenciales incorrectas o usuario inactivo.");
                return View(vm);
            }

            Session["IdUsuario"] = usuario.IdUsuario;
            Session["NombreUsuario"] = usuario.NombreCompleto;
            Session["TipoUsuario"] = usuario.TipoUsuario;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}