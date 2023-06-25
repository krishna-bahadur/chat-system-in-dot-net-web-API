using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.DTOs
{
    public class ServiceResultDTO<T> where T : class
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string[] Errors { get; set; }

        public ServiceResultDTO(bool success, T data = default, string[] errors = null)
        {
            Success = success;
            Data = data;
            Errors = errors;
        }
    }
}
