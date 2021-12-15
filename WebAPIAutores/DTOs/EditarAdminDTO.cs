using System.ComponentModel.DataAnnotations;

namespace WebAPIAutores.DTOs
{
    public class EditarAdminDTO
    {
        [Required]
        [EmailAddress] //Verifica que sea un email valido
        public string Email { get; set; }
    }
}
