using System.ComponentModel.DataAnnotations;

namespace PIV_PF_ProyectoFinal.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "La identificacion o correo es obligatorio.")]
        [Display(Name = "Identificacion o correo")]
        public string Identificacion { get; set; }

        [Required(ErrorMessage = "La contrasena es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contrasena")]
        public string Contrasena { get; set; }
    }
}