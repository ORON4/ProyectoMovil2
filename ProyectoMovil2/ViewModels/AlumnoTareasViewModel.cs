using System.Collections.ObjectModel;
using System.Windows.Input;
using ProyectoMovil2.Models;
using ProyectoMovil2.Services;

namespace ProyectoMovil2.ViewModels
{
    [QueryProperty(nameof(AlumnoId), "alumnoId")]
    [QueryProperty(nameof(NombreAlumno), "nombre")]
    public class AlumnoTareasViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private int _alumnoId;
        private string _nombreAlumno;
        private bool _isRefreshing;

        public AlumnoTareasViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Tareas = new ObservableCollection<Tarea>();
            
            RefreshCommand = new Command(async () => await CargarTareasAsync());
            MarcarEntregadaCommand = new Command<Tarea>(async (t) => await MarcarEntregadaAsync(t));
            EliminarTareaCommand = new Command<Tarea>(async (t) => await EliminarTareaAsync(t));
        }

        public ObservableCollection<Tarea> Tareas { get; }

        public int AlumnoId
        {
            get => _alumnoId;
            set
            {
                _alumnoId = value;
                Task.Run(async () => await CargarTareasAsync());
            }
        }

        public string NombreAlumno
        {
            get => _nombreAlumno;
            set
            {
                _nombreAlumno = value;
                Title = $"Tareas de {value}";
                OnPropertyChanged();
            }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand MarcarEntregadaCommand { get; }
        public ICommand EliminarTareaCommand { get; }

        private async Task CargarTareasAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // RUTA NUEVA: Obtener tareas por ID de alumno
                var lista = await _apiService.GetAsync<List<Tarea>>($"tarea/alumno/{AlumnoId}");
                
                Tareas.Clear();
                if (lista != null)
                {
                    foreach (var t in lista) Tareas.Add(t);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        private async Task MarcarEntregadaAsync(Tarea tarea)
        {
            if (tarea.Estatus) return; // Ya entregada

            bool confirm = await Application.Current.MainPage.DisplayAlert("Entregar", $"¿Marcar '{tarea.Titulo}' como entregada?", "Sí", "Cancelar");
            if (!confirm) return;

            try
            {
                // RUTA NUEVA: Usamos AlumnoTareaId
                await _apiService.PutAsync<object>($"tarea/entregar/{tarea.AlumnoTareaId}", null);
                
                // Actualizar UI localmente
                tarea.Estatus = true;
                // Truco para refrescar el item en la lista (reemplazarlo)
                int index = Tareas.IndexOf(tarea);
                Tareas[index] = tarea; 
                await CargarTareasAsync(); // Recarga completa para asegurar colores
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo marcar como entregada.", "OK");
            }
        }

        private async Task EliminarTareaAsync(Tarea tarea)
        {
            if (tarea.Estatus)
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "No puedes eliminar tareas ya entregadas.", "OK");
                return;
            }

            bool confirm = await Application.Current.MainPage.DisplayAlert("Ocultar", $"¿Quitar esta tarea de la lista de {NombreAlumno}?", "Sí", "No");
            if (!confirm) return;

            try
            {
                // RUTA NUEVA: Eliminar asignación
                await _apiService.DeleteAsync<object>($"tarea/asignacion/{tarea.AlumnoTareaId}");
                Tareas.Remove(tarea);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Tarea eliminada correctamente.", "OK");
            }
        }
    }
}