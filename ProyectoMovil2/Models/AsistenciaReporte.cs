using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoMovil2.Models
{
    public class AsistenciaReporte
    {
        public int Id { get; set; }
        public int IdAlumno { get; set; }
        public string NombreAlumno { get; set; }
        public DateTime Fecha { get; set; }
        public bool Asistencia { get; set; }
    }
}
