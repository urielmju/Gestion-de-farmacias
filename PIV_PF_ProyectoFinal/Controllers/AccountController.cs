using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using PIV_PF_ProyectoFinal.Models;
using PIV_PF_ProyectoFinal.Seguridad;
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
            Usuarios usuario;

            try
            {
                usuario = db.Usuarios
                            .FirstOrDefault(u =>
                                (u.Identificacion == credencial || u.Correo == credencial)
                                && u.Estado == "Activo");
            }
            catch (Exception ex)
            {
                RegistrarError(ex);
                ModelState.AddModelError("",
                    "La base de datos esta reactivandose, intenta de nuevo en unos segundos.");
                return View(vm);
            }

            if (usuario == null || !HashContrasena.Verificar(vm.Contrasena, usuario.Contrasena))
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

        private static readonly string[] RolesAutoRegistrables = { "Vendedor", "Surtidor", "Contador" };

        public ActionResult Registro()
        {
            var vm = new UsuarioViewModel();
            ViewBag.TiposUsuario = RolesAutoRegistrables;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(UsuarioViewModel vm)
        {
            ViewBag.TiposUsuario = RolesAutoRegistrables;

            if (!RolesAutoRegistrables.Contains(vm.TipoUsuario))
                ModelState.AddModelError("TipoUsuario", "Tipo de usuario invalido.");

            if (string.IsNullOrEmpty(vm.Contrasena))
            {
                ModelState.AddModelError("Contrasena", "La contrasena es obligatoria.");
            }
            else
            {
                bool tieneNumero = Regex.IsMatch(vm.Contrasena, @"\d");
                bool tieneEspecial = Regex.IsMatch(vm.Contrasena, @"[!@$%^*()\-_=+\[\]{}:;.,?|~]");
                if (vm.Contrasena.Length < 6 || !tieneNumero || !tieneEspecial)
                    ModelState.AddModelError("Contrasena",
                        "La contrasena debe tener minimo 6 caracteres, un numero y un caracter especial.");
            }

            ModelState.Remove("Estado");

            if (!ModelState.IsValid)
                return View(vm);

            bool identRepetida = db.Usuarios.Any(u => u.Identificacion == vm.Identificacion);
            if (identRepetida)
            {
                ModelState.AddModelError("Identificacion", "Ya existe un usuario con esta identificacion.");
                return View(vm);
            }

            var usuario = new Usuarios
            {
                Identificacion = vm.Identificacion.Trim(),
                NombreCompleto = vm.NombreCompleto.Trim(),
                Correo = vm.Correo.Trim(),
                TipoUsuario = vm.TipoUsuario,
                Estado = "Activo",
                Contrasena = HashContrasena.Generar(vm.Contrasena),
                FechaRegistro = DateTime.Now
            };

            db.Usuarios.Add(usuario);
            db.SaveChanges();

            TempData["Success"] = "Cuenta creada exitosamente. Ya puedes iniciar sesion.";
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        private static void RegistrarError(Exception ex)
        {
            var texto = string.Format(
                "{0:yyyy-MM-dd HH:mm:ss} UTC{1}Login - excepcion atrapada{1}{2}{1}{3}{1}",
                DateTime.UtcNow, Environment.NewLine, ex.ToString(), new string('-', 60));

            var home = Environment.GetEnvironmentVariable("HOME");
            if (!string.IsNullOrEmpty(home))
            {
                try
                {
                    var ruta = Path.Combine(home, "LogFiles", "error_log.txt");
                    File.AppendAllText(ruta, texto + Environment.NewLine);
                    return;
                }
                catch
                {
                }
            }

            try
            {
                var ruta = HttpContext.Current.Server.MapPath("~/App_Data/error_log.txt");
                File.AppendAllText(ruta, texto + Environment.NewLine);
            }
            catch
            {
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}