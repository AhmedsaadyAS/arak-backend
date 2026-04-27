using System.ComponentModel.DataAnnotations;

namespace Arak.BLL.DTO
{
    public class DtoSendMessage
    {
        [Required]
        [MaxLength(4000)]
        public string Content { get; set; }
    }
}
