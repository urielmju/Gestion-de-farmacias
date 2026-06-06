using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PIV_PF_ProyectoFinal.Filters;
using PIV_PF_ProyectoFinal.Models;
using PIV_PF_ProyectoFinal.ViewModels;

namespace PIV_PF_ProyectoFinal.Controllers
{
    [SessionAuthorize(Roles = "Administrador,Vendedor,Surtidor,Contador")]
    public class ProductosController : Controller
    {
        private PIV_PF_ProyectoFinalEntities1 db = new PIV_PF_ProyectoFinalEntities1();

        public ProductosController()
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        private static readonly List<SelectListItem> OpcionesEstado = new List<SelectListItem>
        {
            new SelectListItem { Value = "En existencia", Text = "En existencia" },
            new SelectListItem { Value = "Agotado",       Text = "Agotado"       }
        };

        private void CargarDropdowns(string codigoTipoSeleccionado = null, string estadoSeleccionado = null)
        {
            var tipos = db.TiposProducto
                          .OrderBy(t => t.Descripcion)
                          .Select(t => new SelectListItem
                          {
                              Value = t.CodigoTipo,
                              Text = t.Descripcion
                          })
                          .ToList();

            ViewBag.TiposProducto = new SelectList(tipos, "Value", "Text", codigoTipoSeleccionado);
            ViewBag.Estados = new SelectList(OpcionesEstado, "Value", "Text", estadoSeleccionado);
        }

        private string GenerarCodigoProducto()
        {
            var codigos = db.Productos.Select(p => p.CodigoProducto).ToList();
            int max = 0;
            foreach (var c in codigos)
                if (c != null && c.StartsWith("P") && c.Length > 1)
                    if (int.TryParse(c.Substring(1), out int n) && n > max)
                        max = n;
            return "P" + (max + 1).ToString("D3");
        }

        // GET: Productos — todos los roles
        public ActionResult Index()
        {
            var productos = db.Productos
                              .OrderBy(p => p.Descripcion)
                              .Select(p => new ProductoViewModel
                              {
                                  CodigoProducto = p.CodigoProducto,
                                  Descripcion = p.Descripcion,
                                  Precio = p.Precio,
                                  Cantidad = p.Cantidad,
                                  Estado = p.Estado,
                                  CodigoTipo = p.CodigoTipo,
                                  DescripcionTipo = p.TiposProducto.Descripcion
                              })
                              .ToList();

            return View(productos);
        }

        // GET: Productos/Details — todos los roles
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");

            var vm = db.Productos
                       .Where(p => p.CodigoProducto == id)
                       .Select(p => new ProductoViewModel
                       {
                           CodigoProducto = p.CodigoProducto,
                           Descripcion = p.Descripcion,
                           Precio = p.Precio,
                           Cantidad = p.Cantidad,
                           Estado = p.Estado,
                           CodigoTipo = p.CodigoTipo,
                           DescripcionTipo = p.TiposProducto.Descripcion
                       })
                       .FirstOrDefault();

            if (vm == null) return HttpNotFound();
            return View(vm);
        }

        // GET: Productos/Create — solo Administrador
        [SessionAuthorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            CargarDropdowns();
            return View(new ProductoViewModel
            {
                CodigoProducto = GenerarCodigoProducto(),
                Estado = "En existencia"
            });
        }

        // POST: Productos/Create — solo Administrador
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(Roles = "Administrador")]
        public ActionResult Create(ProductoViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                CargarDropdowns(vm.CodigoTipo, vm.Estado);
                return View(vm);
            }

            bool codigoRepetido = db.Productos.Any(p => p.CodigoProducto == vm.CodigoProducto);
            if (codigoRepetido)
            {
                ModelState.AddModelError("CodigoProducto", "Ya existe un producto con este codigo.");
                CargarDropdowns(vm.CodigoTipo, vm.Estado);
                return View(vm);
            }

            string estadoFinal = vm.Cantidad == 0 ? "Agotado" : "En existencia";

            db.Productos.Add(new Productos
            {
                CodigoProducto = vm.CodigoProducto.Trim().ToUpper(),
                Descripcion = vm.Descripcion.Trim(),
                Precio = vm.Precio,
                Cantidad = vm.Cantidad,
                Estado = estadoFinal,
                CodigoTipo = vm.CodigoTipo
            });

            db.SaveChanges();
            TempData["Success"] = "Producto registrado correctamente.";
            return RedirectToAction("Index");
        }

        // GET: Productos/Edit — solo Administrador
        [SessionAuthorize(Roles = "Administrador")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");

            var vm = db.Productos
                       .Where(p => p.CodigoProducto == id)
                       .Select(p => new ProductoViewModel
                       {
                           CodigoProducto = p.CodigoProducto,
                           Descripcion = p.Descripcion,
                           Precio = p.Precio,
                           Cantidad = p.Cantidad,
                           Estado = p.Estado,
                           CodigoTipo = p.CodigoTipo,
                           DescripcionTipo = p.TiposProducto.Descripcion
                       })
                       .FirstOrDefault();

            if (vm == null) return HttpNotFound();
            CargarDropdowns(vm.CodigoTipo, vm.Estado);
            return View(vm);
        }

        // POST: Productos/Edit — solo Administrador
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(Roles = "Administrador")]
        public ActionResult Edit(ProductoViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                CargarDropdowns(vm.CodigoTipo, vm.Estado);
                return View(vm);
            }

            Productos producto = db.Productos.Find(vm.CodigoProducto);
            if (producto == null) return HttpNotFound();

            string estadoFinal = vm.Cantidad == 0 ? "Agotado" : vm.Estado;
            producto.Descripcion = vm.Descripcion.Trim();
            producto.Precio = vm.Precio;
            producto.Cantidad = vm.Cantidad;
            producto.Estado = estadoFinal;
            producto.CodigoTipo = vm.CodigoTipo;

            db.SaveChanges();
            TempData["Success"] = "Producto actualizado correctamente.";
            return RedirectToAction("Index");
        }

        // GET: Productos/Surtir — Administrador y Contabilidad
        [SessionAuthorize(Roles = "Administrador,Surtidor")]
        public ActionResult Surtir(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");

            var producto = db.Productos.Find(id);
            if (producto == null) return HttpNotFound();

            return View(new SurtirViewModel
            {
                CodigoProducto = producto.CodigoProducto,
                Descripcion = producto.Descripcion,
                CantidadActual = producto.Cantidad
            });
        }

        // POST: Productos/Surtir — Administrador y Contabilidad
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(Roles = "Administrador,Surtidor")]
        public ActionResult Surtir(SurtirViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            Productos producto = db.Productos.Find(vm.CodigoProducto);
            if (producto == null) return HttpNotFound();

            producto.Cantidad += vm.CantidadAgregar;
            producto.Estado = "En existencia";

            db.SaveChanges();
            TempData["Success"] = $"Se agregaron {vm.CantidadAgregar} unidades a {producto.Descripcion}.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}