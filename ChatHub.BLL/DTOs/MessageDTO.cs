using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.DTOs
{
    public class MessageDTO
    {
        public string? MessageId { get; set; }
        public string? Messages { get; set; }
        public string? SenderUsername { get; set; }
        public string? ReceiverUsername { get; set; }
        public string? FileURL { get; set; }
        public string? DepartmentId { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsDeletedBySender { get; set; }
        public bool IsDeleted { get; set; }
    }
}
