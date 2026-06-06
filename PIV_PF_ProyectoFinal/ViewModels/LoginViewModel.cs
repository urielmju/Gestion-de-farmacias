using System.ComponentModel.DataAnnotations;

namespace PIV_PF_ProyectoFinal.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "La identificacion o correo es obligatorio.")]
        [Display(Name = "Identificacion o correo")]
        public string Identificacion { get; set; }

        [Required(ErrorMessage = "La contrasena es obligatoria.")]
        [RegularExpression(@"^(?=.*\d)(?=.*[!@#$%^&*()\-_=+\[\]{};:'"",.<>?\\|`~]).{6,}$",
            ErrorMessage = "La contrasena debe tener minimo 6 caracteres, un numero y un caracter especial.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contrasena")]
        public string Contrasena { get; set; }
    }
}