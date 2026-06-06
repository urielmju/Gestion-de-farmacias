using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PIV_PF_ProyectoFinal.Filters;
using PIV_PF_ProyectoFinal.Models;
using PIV_PF_ProyectoFinal.ViewModels;

namespace PIV_PF_ProyectoFinal.Controllers
{
    [SessionAuthorize(Roles = "Administrador,Vendedor,Surtidor,Contador")]
    public class FacturasController : Controller
    {
        private PIV_PF_ProyectoFinalEntities1 db = new PIV_PF_ProyectoFinalEntities1();

        public FacturasController()
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        private string GenerarCodigoFactura()
        {
            var codigos = db.Facturas.Select(f => f.CodigoFactura).ToList();
            int max = 0;
            foreach (var c in codigos)
            {
                if (c != null && c.StartsWith("FAC") && c.Length > 3)
                    if (int.TryParse(c.Substring(3), out int n) && n > max)
                        max = n;
            }
            return "FAC" + (max + 1).ToString("D4");
        }

        // GET: Facturas
        public ActionResult Index()
        {
            var facturas = db.Facturas
                             .OrderByDescending(f => f.Fecha)
                             .Select(f => new FacturaViewModel
                             {
                                 CodigoFactura = f.CodigoFactura,
                                 Fecha = f.Fecha,
                                 IdCliente = f.IdCliente,
                                 NombreCliente = f.Clientes.NombreCompleto,
                                 MetodoPago = f.MetodoPago,
                                 Subtotal = f.Subtotal,
                                 Recargo = f.Recargo,
                                 Total = f.Total
                             })
                             .ToList();

            return View(facturas);
        }

        // GET: Facturas/Details/FAC0001
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");

            var factura = db.Facturas
                            .Where(f => f.CodigoFactura == id)
                            .Select(f => new FacturaViewModel
                            {
                                CodigoFactura = f.CodigoFactura,
                                Fecha = f.Fecha,
                                IdCliente = f.IdCliente,
                                NombreCliente = f.Clientes.NombreCompleto,
                                MetodoPago = f.MetodoPago,
                                Subtotal = f.Subtotal,
                                Recargo = f.Recargo,
                                Total = f.Total
                            })
                            .FirstOrDefault();

            if (factura == null)
                return HttpNotFound();

            factura.Detalles = db.DetalleFactura
                                 .Where(d => d.CodigoFactura == id)
                                 .Select(d => new DetalleFacturaViewModel
                                 {
                                     CodigoProducto = d.CodigoProducto,
                                     DescripcionProducto = d.Productos.Descripcion,
                                     PrecioUnitario = d.PrecioUnitario,
                                     Cantidad = d.Cantidad,
                                     SubtotalLinea = d.SubtotalLinea
                                 })
                                 .ToList();

            return View(factura);
        }

        // GET: Facturas/Create
        [SessionAuthorize(Roles = "Administrador,Vendedor,Contador")]
        public ActionResult Create()
        {
            CargarDropdownsFactura();
            return View(new FacturaViewModel
            {
                CodigoFactura = GenerarCodigoFactura(),
                Fecha = DateTime.Now
            });
        }

        // POST: Facturas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(Roles = "Administrador,Vendedor,Contador")]
        public ActionResult Create(FacturaViewModel vm, string[] codigos, int[] cantidades)
        {
            // armar detalles desde los arrays del form
            vm.Detalles = new List<DetalleFacturaViewModel>();

            if (codigos != null && cantidades != null)
            {
                for (int i = 0; i < codigos.Length; i++)
                {
                    if (string.IsNullOrEmpty(codigos[i])) continue;

                    var producto = db.Productos.Find(codigos[i]);
                    if (producto == null) continue;

                    int cant = i < cantidades.Length ? cantidades[i] : 0;
                    if (cant <= 0) continue;

                    vm.Detalles.Add(new DetalleFacturaViewModel
                    {
                        CodigoProducto = producto.CodigoProducto,
                        DescripcionProducto = producto.Descripcion,
                        PrecioUnitario = producto.Precio,
                        Cantidad = cant,
                        SubtotalLinea = producto.Precio * cant
                    });
                }
            }

            if (vm.Detalles.Count == 0)
            {
                ModelState.AddModelError("", "Debe agregar al menos un producto a la factura.");
                CargarDropdownsFactura();
                return View(vm);
            }

            // validar stock de cada producto
            foreach (var detalle in vm.Detalles)
            {
                var producto = db.Productos.Find(detalle.CodigoProducto);
                if (producto.Estado == "Agotado" || producto.Cantidad <= 0)
                {
                    ModelState.AddModelError("",
                        $"El producto '{producto.Descripcion}' esta agotado y no puede venderse.");
                    CargarDropdownsFactura();
                    return View(vm);
                }
                if (detalle.Cantidad > producto.Cantidad)
                {
                    ModelState.AddModelError("",
                        $"Stock insuficiente para '{producto.Descripcion}'. Disponible: {producto.Cantidad}.");
                    CargarDropdownsFactura();
                    return View(vm);
                }
            }

            // calcular totales
            decimal subtotal = vm.Detalles.Sum(d => d.SubtotalLinea);
            decimal recargo = vm.MetodoPago == "Tarjeta" ? Math.Round(subtotal * 0.02m, 2) : 0;
            decimal total = subtotal + recargo;

            // guardar factura
            var factura = new Facturas
            {
                CodigoFactura = vm.CodigoFactura.Trim().ToUpper(),
                Fecha = DateTime.Now,
                IdCliente = vm.IdCliente,
                MetodoPago = vm.MetodoPago,
                Subtotal = subtotal,
                Recargo = recargo,
                Total = total
            };

            db.Facturas.Add(factura);

            // guardar detalles y reducir inventario
            foreach (var detalle in vm.Detalles)
            {
                db.DetalleFactura.Add(new DetalleFactura
                {
                    CodigoFactura = factura.CodigoFactura,
                    CodigoProducto = detalle.CodigoProducto,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = detalle.PrecioUnitario,
                    SubtotalLinea = detalle.SubtotalLinea
                });

                // reducir inventario
                var producto = db.Productos.Find(detalle.CodigoProducto);
                producto.Cantidad -= detalle.Cantidad;
                if (producto.Cantidad <= 0)
                {
                    producto.Cantidad = 0;
                    producto.Estado = "Agotado";
                }
            }

            db.SaveChanges();

            TempData["Success"] = $"Factura {factura.CodigoFactura} registrada correctamente.";
            return RedirectToAction("Details", new { id = factura.CodigoFactura });
        }
        // GET: Facturas/GetProducto?id=P001 — llamado por AJAX
        public JsonResult GetProducto(string id)
        {
            var p = db.Productos
                      .Where(x => x.CodigoProducto == id)
                      .Select(x => new {
                          x.CodigoProducto,
                          x.Descripcion,
                          x.Precio,
                          x.Cantidad,
                          x.Estado
                      })
                      .FirstOrDefault();

            if (p == null)
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);

            return Json(new { ok = true, producto = p }, JsonRequestBehavior.AllowGet);
        }

        // GET: Facturas/GetClientes — para el dropdown
        private void CargarDropdownsFactura()
        {
            ViewBag.Clientes = new SelectList(
                db.Clientes.OrderBy(c => c.NombreCompleto)
                           .Select(c => new { c.IdCliente, c.NombreCompleto }),
                "IdCliente", "NombreCompleto");

            ViewBag.Productos = db.Productos
                                  .Where(p => p.Estado == "En existencia" && p.Cantidad > 0)
                                  .OrderBy(p => p.Descripcion)
                                  .Select(p => new SelectListItem
                                  {
                                      Value = p.CodigoProducto,
                                      Text = p.Descripcion + " (₡" + p.Precio + ")"
                                  })
                                  .ToList();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}