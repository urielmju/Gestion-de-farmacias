using System.ComponentModel.DataAnnotations;

namespace PIV_PF_ProyectoFinal.ViewModels
{
    public class ProductoViewModel
    {
        [Required(ErrorMessage = "El codigo del producto es obligatorio.")]
        [StringLength(20, ErrorMessage = "El codigo no puede superar los 20 caracteres.")]
        [Display(Name = "Codigo del producto")]
        public string CodigoProducto { get; set; }

        [Required(ErrorMessage = "La descripcion es obligatoria.")]
        [StringLength(200, ErrorMessage = "La descripcion no puede superar los 200 caracteres.")]
        [Display(Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, 9999999.99, ErrorMessage = "El precio debe ser mayor a cero.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Precio")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
        [Display(Name = "Cantidad en existencia")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [Required(ErrorMessage = "El tipo de producto es obligatorio.")]
        [Display(Name = "Tipo de producto")]
        public string CodigoTipo { get; set; }

        // solo para mostrar en Index y Details
        public string DescripcionTipo { get; set; }
    }
}
