using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProyectoMovil2.ViewModels
{
    public class GrupoViewModel : BaseViewModel
    {
        public GrupoViewModel()
        {
            Title = "Grupo";
            IrATareasCommand = new Command(async () => await IrATareas());
            // --- AÑADE EL COMANDO PARA ASISTENCIAS ---
            IrAAsistenciasCommand = new Command(async () => await IrAAsistencias());
        }

        public ICommand IrATareasCommand { get; }
        // --- AÑADE ESTA PROPIEDAD ---
        public ICommand IrAAsistenciasCommand { get; }

        private async Task IrATareas()
        {
            // --- CORRECCIÓN: Usa ruta absoluta (con //) ---
            await Shell.Current.GoToAsync("TareasPage");
        }

        // --- AÑADE ESTE MÉTODO ---
        private async Task IrAAsistencias()
        {
            // --- CORRECCIÓN: Usa ruta absoluta (con //) ---
            await Shell.Current.GoToAsync("AsistenciaPage");
        }
    }
}