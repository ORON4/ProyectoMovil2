using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoMovil2.Models
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Mensaje { get; set; }
    }
}
