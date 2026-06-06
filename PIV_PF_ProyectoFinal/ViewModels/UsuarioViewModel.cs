using System;
using System.ComponentModel.DataAnnotations;

namespace PIV_PF_ProyectoFinal.ViewModels
{
    public class UsuarioViewModel
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "La identificacion es obligatoria.")]
        [StringLength(20, ErrorMessage = "La identificacion no puede superar los 20 caracteres.")]
        [Display(Name = "Identificacion")]
        public string Identificacion { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$",
            ErrorMessage = "El nombre no debe contener numeros ni caracteres especiales.")]
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El correo electronico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es valido.")]
        [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
        [Display(Name = "Correo electronico")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El tipo de usuario es obligatorio.")]
        [Display(Name = "Tipo de usuario")]
        public string TipoUsuario { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistro { get; set; }
        [Display(Name = "Contrasena")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }
    }
}