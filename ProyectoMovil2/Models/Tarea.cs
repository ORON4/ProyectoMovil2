using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoMovil2.Models
{
    public class Tarea
    {
        public int Id { get; set; }
        public int AlumnoTareaId { get; set; }  
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaEntrega { get; set; }
        public bool Estatus { get; set; }

        // Propiedades calculadas para la UI
        public string EstatusTexto => Estatus ? "Completada" : "Pendiente";
        public Color EstatusColor => Estatus ? Colors.Green : Colors.Orange;
        public string FechaEntregaTexto => FechaEntrega.ToString("dd/MM/yyyy");
    }
}
    

