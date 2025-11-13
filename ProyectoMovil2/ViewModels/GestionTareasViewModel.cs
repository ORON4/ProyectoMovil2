using System.Collections.ObjectModel;
using System.Windows.Input;
using ProyectoMovil2.Models;
using ProyectoMovil2.Services;

namespace ProyectoMovil2.ViewModels
{
    public class GestionTareasViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private bool _isRefreshing;

        public GestionTareasViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Gestión de Tareas";
            TareasGlobales = new ObservableCollection<Tarea>();

            RefreshCommand = new Command(async () => await CargarGlobalesAsync());
            EditarCommand = new Command<Tarea>(async (t) => await EditarAsync(t));
            EliminarCommand = new Command<Tarea>(async (t) => await EliminarAsync(t));
            NuevaTareaCommand = new Command(async () => await NuevaTareaAsync());
        }

        public ObservableCollection<Tarea> TareasGlobales { get; }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand NuevaTareaCommand { get; }

        public async Task InitializeAsync()
        {
            await CargarGlobalesAsync();
        }

        private async Task CargarGlobalesAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var lista = await _apiService.ObtenerTareasGlobalesAsync();
                TareasGlobales.Clear();
                if (lista != null)
                {
                    // Ordenamos por fecha de entrega
                    foreach (var t in lista.OrderBy(x => x.FechaEntrega))
                        TareasGlobales.Add(t);
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

        private async Task NuevaTareaAsync()
        {
            await Shell.Current.GoToAsync("CrearEditarTareaPage");
        }

        private async Task EditarAsync(Tarea tarea)
        {
            // Navega a la página de edición pasando el ID global
            await Shell.Current.GoToAsync($"CrearEditarTareaPage?tareaId={tarea.Id}");
        }

        private async Task EliminarAsync(Tarea tarea)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "⚠️ ADVERTENCIA",
                $"Si eliminas '{tarea.Titulo}', desaparecerá de TODOS los alumnos.\n\n¿Estás seguro?",
                "Sí, Eliminar", "Cancelar");

            if (!confirm) return;

            try
            {
                // Llamamos al endpoint DELETE /tarea/{id}
                await _apiService.DeleteAsync<object>($"tarea/{tarea.Id}");

                TareasGlobales.Remove(tarea);
                await Application.Current.MainPage.DisplayAlert("Éxito", "Tarea eliminada globalmente.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Tarea eliminada.", "OK");
            }
        }
    }
}