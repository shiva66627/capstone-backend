using CapStone.Models;
using System.ComponentModel.DataAnnotations;

namespace CapStone.DTO.User
{
    public class UserStatusUpdateDTO
    {
        [Required]
        public ActiveStatusEnum ActiveStatus { get; set; }
    }
}