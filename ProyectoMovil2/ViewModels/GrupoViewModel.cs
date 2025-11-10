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

            IrAEscanerCommand = new Command(async () => await IrAEscaner());
        }

        public ICommand IrATareasCommand { get; }
        // --- AÑADE ESTA PROPIEDAD ---
        public ICommand IrAAsistenciasCommand { get; }

        public ICommand IrAEscanerCommand { get; }

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

        private async Task IrAEscaner()
        {
            // Pide permiso para la cámara antes de navegar
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status == PermissionStatus.Granted)
            {
                await Shell.Current.GoToAsync("EscanerQRpage");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Permiso Requerido",
                    "Se necesita permiso de la cámara para escanear.", "OK");
            }
        }
    }
}