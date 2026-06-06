using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PIV_PF_ProyectoFinal.ViewModels
{
    public class DetalleFacturaViewModel
    {
        [Required]
        public string CodigoProducto { get; set; }
        public string DescripcionProducto { get; set; }
        public decimal PrecioUnitario { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int Cantidad { get; set; }

        public decimal SubtotalLinea { get; set; }
    }

    public class FacturaViewModel
    {
        public string CodigoFactura { get; set; }
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El cliente es obligatorio.")]
        [Display(Name = "Cliente")]
        public int IdCliente { get; set; }

        public string NombreCliente { get; set; }

        [Required(ErrorMessage = "El metodo de pago es obligatorio.")]
        [Display(Name = "Metodo de pago")]
        public string MetodoPago { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Recargo { get; set; }
        public decimal Total { get; set; }

        public List<DetalleFacturaViewModel> Detalles { get; set; }
            = new List<DetalleFacturaViewModel>();
    }
}