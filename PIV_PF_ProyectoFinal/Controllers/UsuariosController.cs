using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PIV_PF_ProyectoFinal.Filters;
using PIV_PF_ProyectoFinal.Models;
using PIV_PF_ProyectoFinal.ViewModels;

namespace PIV_PF_ProyectoFinal.Controllers
{
    [SessionAuthorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private PIV_PF_ProyectoFinalEntities1 db = new PIV_PF_ProyectoFinalEntities1();

        public UsuariosController()
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        private static readonly List<SelectListItem> OpcionesTipo = new List<SelectListItem>
        {
          new SelectListItem { Value = "Administrador", Text = "Administrador" },
    new SelectListItem { Value = "Vendedor",      Text = "Vendedor"      },
    new SelectListItem { Value = "Surtidor",      Text = "Surtidor"      },
    new SelectListItem { Value = "Contador",      Text = "Contador"      }
        };

        private static readonly List<SelectListItem> OpcionesEstado = new List<SelectListItem>
        {
            new SelectListItem { Value = "Activo",   Text = "Activo"   },
            new SelectListItem { Value = "Inactivo", Text = "Inactivo" }
        };

        private void CargarDropdowns(string tipoSeleccionado = null, string estadoSeleccionado = null)
        {
            ViewBag.TiposUsuario = new SelectList(OpcionesTipo, "Value", "Text", tipoSeleccionado);
            ViewBag.Estados = new SelectList(OpcionesEstado, "Value", "Text", estadoSeleccionado);
        }

        private string GenerarCodigoEmpleado()
        {
            var codigos = db.Usuarios.Select(u => u.Identificacion).ToList();
            int max = 0;
            foreach (var c in codigos)
            {
                if (c != null && c.StartsWith("EMP") && c.Length > 3)
                    if (int.TryParse(c.Substring(3), out int n) && n > max)
                        max = n;
            }
            return "EMP" + (max + 1).ToString("D3");
        }

        public ActionResult Index()
        {
            var usuarios = db.Usuarios
                             .OrderBy(u => u.NombreCompleto)
                             .Select(u => new UsuarioViewModel
                             {
                                 IdUsuario = u.IdUsuario,
                                 Identificacion = u.Identificacion,
                                 NombreCompleto = u.NombreCompleto,
                                 Correo = u.Correo,
                                 TipoUsuario = u.TipoUsuario,
                                 Estado = u.Estado,
                                 FechaRegistro = u.FechaRegistro
                             })
                             .ToList();
            return View(usuarios);
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return RedirectToAction("Index");

            var vm = db.Usuarios
                       .Where(u => u.IdUsuario == id)
                       .Select(u => new UsuarioViewModel
                       {
                           IdUsuario = u.IdUsuario,
                           Identificacion = u.Identificacion,
                           NombreCompleto = u.NombreCompleto,
                           Correo = u.Correo,
                           TipoUsuario = u.TipoUsuario,
                           Estado = u.Estado,
                           FechaRegistro = u.FechaRegistro
                       })
                       .FirstOrDefault();

            if (vm == null) return HttpNotFound();
            return View(vm);
        }

        public ActionResult Create()
        {
            CargarDropdowns();
            return View(new UsuarioViewModel { Identificacion = GenerarCodigoEmpleado() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UsuarioViewModel vm)
        {
            // validar contrasena manualmente porque en Edit no es requerida
            if (string.IsNullOrEmpty(vm.Contrasena))
                ModelState.AddModelError("Contrasena", "La contrasena es obligatoria.");
            else
            {
                bool tieneNumero = System.Text.RegularExpressions.Regex.IsMatch(vm.Contrasena, @"\d");
                bool tieneEspecial = System.Text.RegularExpressions.Regex.IsMatch(vm.Contrasena, @"[!@#$%^&*()\-_=+\[\]{};:'"",.<>?\\|`~]");
                if (vm.Contrasena.Length < 6 || !tieneNumero || !tieneEspecial)
                    ModelState.AddModelError("Contrasena",
                        "La contrasena debe tener minimo 6 caracteres, un numero y un caracter especial.");
            }

            if (!ModelState.IsValid)
            {
                CargarDropdowns(vm.TipoUsuario, vm.Estado);
                return View(vm);
            }

            bool identRepetida = db.Usuarios.Any(u => u.Identificacion == vm.Identificacion);
            if (identRepetida)
            {
                ModelState.AddModelError("Identificacion", "Ya existe un usuario con esta identificacion.");
                CargarDropdowns(vm.TipoUsuario, vm.Estado);
                return View(vm);
            }

            var usuario = new Usuarios
            {
                Identificacion = vm.Identificacion.Trim(),
                NombreCompleto = vm.NombreCompleto.Trim(),
                Correo = vm.Correo.Trim(),
                TipoUsuario = vm.TipoUsuario,
                Estado = vm.Estado,
                Contrasena = vm.Contrasena,
                FechaRegistro = DateTime.Now
            };

            db.Usuarios.Add(usuario);
            db.SaveChanges();

            TempData["Success"] = "Usuario registrado correctamente.";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            if (id == null) return RedirectToAction("Index");

            var vm = db.Usuarios
                       .Where(u => u.IdUsuario == id)
                       .Select(u => new UsuarioViewModel
                       {
                           IdUsuario = u.IdUsuario,
                           Identificacion = u.Identificacion,
                           NombreCompleto = u.NombreCompleto,
                           Correo = u.Correo,
                           TipoUsuario = u.TipoUsuario,
                           Estado = u.Estado,
                           FechaRegistro = u.FechaRegistro
                       })
                       .FirstOrDefault();

            if (vm == null) return HttpNotFound();
            CargarDropdowns(vm.TipoUsuario, vm.Estado);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UsuarioViewModel vm)
        {
            // quitar validacion de Contrasena en Edit
            ModelState.Remove("Contrasena");

            if (!ModelState.IsValid)
            {
                CargarDropdowns(vm.TipoUsuario, vm.Estado);
                return View(vm);
            }

            Usuarios usuario = db.Usuarios.Find(vm.IdUsuario);
            if (usuario == null) return HttpNotFound();

            usuario.NombreCompleto = vm.NombreCompleto.Trim();
            usuario.Correo = vm.Correo.Trim();
            usuario.TipoUsuario = vm.TipoUsuario;
            usuario.Estado = vm.Estado;

            db.SaveChanges();

            TempData["Success"] = "Usuario actualizado correctamente.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}