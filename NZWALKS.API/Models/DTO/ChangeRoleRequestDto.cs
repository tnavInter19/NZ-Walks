using System.ComponentModel.DataAnnotations;

namespace NZWALKS.API.Models.DTO
{
    public class ChangeRoleRequestDto
    {
        [Required]
        public string[] Roles { get; set; }
    }
}
