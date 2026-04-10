using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.BLL.DTO
{
    public class DtoStudent
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public bool Status { get; set; } = true;
        public string? Image { get; set; }
        public int? ParentId { get; set; }
        public int? ClassId { get; set; }
    }
}
