using System.ComponentModel.DataAnnotations;

namespace PIV_PF_ProyectoFinal.ViewModels
{
    public class SurtirViewModel
    {
        public string CodigoProducto { get; set; }
        public string Descripcion { get; set; }
        public int CantidadActual { get; set; }

        [Required(ErrorMessage = "La cantidad a agregar es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe agregar al menos 1 unidad.")]
        [Display(Name = "Cantidad a agregar")]
        public int CantidadAgregar { get; set; }
    }
}