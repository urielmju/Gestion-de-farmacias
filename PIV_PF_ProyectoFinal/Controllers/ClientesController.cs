using System;
using System.Linq;
using System.Web.Mvc;
using PIV_PF_ProyectoFinal.Filters;
using PIV_PF_ProyectoFinal.Models;
using PIV_PF_ProyectoFinal.ViewModels;

namespace PIV_PF_ProyectoFinal.Controllers
{
    [SessionAuthorize(Roles = "Administrador,Vendedor")]
    public class ClientesController : Controller
    {
        private PIV_PF_ProyectoFinalEntities1 db = new PIV_PF_ProyectoFinalEntities1();

        public ClientesController()
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        // GET: Clientes
        public ActionResult Index()
        {
            var clientes = db.Clientes
                             .OrderBy(c => c.NombreCompleto)
                             .Select(c => new ClienteViewModel
                             {
                                 IdCliente      = c.IdCliente,
                                 Identificacion = c.Identificacion,
                                 NombreCompleto = c.NombreCompleto,
                                 Correo         = c.Correo,
                                 FechaRegistro  = c.FechaRegistro
                             })
                             .ToList();

            return View(clientes);
        }

        // GET: Clientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return RedirectToAction("Index");

            var vm = db.Clientes
                       .Where(c => c.IdCliente == id)
                       .Select(c => new ClienteViewModel
                       {
                           IdCliente      = c.IdCliente,
                           Identificacion = c.Identificacion,
                           NombreCompleto = c.NombreCompleto,
                           Correo         = c.Correo,
                           FechaRegistro  = c.FechaRegistro
                       })
                       .FirstOrDefault();

            if (vm == null)
                return HttpNotFound();

            return View(vm);
        }

        // GET: Clientes/Create
        public ActionResult Create()
        {
            return View(new ClienteViewModel());
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClienteViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            bool identRepetida = db.Clientes.Any(c => c.Identificacion == vm.Identificacion);
            if (identRepetida)
            {
                ModelState.AddModelError("Identificacion",
                    "Ya existe un cliente con esta identificacion.");
                return View(vm);
            }

            var cliente = new Clientes
            {
                Identificacion = vm.Identificacion.Trim(),
                NombreCompleto = vm.NombreCompleto.Trim(),
                Correo         = vm.Correo.Trim(),
                FechaRegistro  = DateTime.Now
            };

            db.Clientes.Add(cliente);
            db.SaveChanges();

            TempData["Success"] = "Cliente registrado correctamente.";
            return RedirectToAction("Index");
        }

        // GET: Clientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("Index");

            var vm = db.Clientes
                       .Where(c => c.IdCliente == id)
                       .Select(c => new ClienteViewModel
                       {
                           IdCliente      = c.IdCliente,
                           Identificacion = c.Identificacion,
                           NombreCompleto = c.NombreCompleto,
                           Correo         = c.Correo,
                           FechaRegistro  = c.FechaRegistro
                       })
                       .FirstOrDefault();

            if (vm == null)
                return HttpNotFound();

            return View(vm);
        }

        // POST: Clientes/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClienteViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            Clientes cliente = db.Clientes.Find(vm.IdCliente);
            if (cliente == null)
                return HttpNotFound();

            cliente.NombreCompleto = vm.NombreCompleto.Trim();
            cliente.Correo         = vm.Correo.Trim();

            db.SaveChanges();

            TempData["Success"] = "Cliente actualizado correctamente.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
