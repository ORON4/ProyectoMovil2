using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ProyectoMovil2.Models;
using ProyectoMovil2.Services;

namespace ProyectoMovil2.ViewModels
{
    public class TareasViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private bool _isRefreshing;
        private string _mensajeError;

        public TareasViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Tareas";

            Tareas = new ObservableCollection<Tarea>();

            CargarTareasCommand = new Command(async () => await CargarTareasAsync());
            RefreshCommand = new Command(async () => await RefreshAsync());
            CrearTareaCommand = new Command(async () => await CrearTareaAsync());
            EditarTareaCommand = new Command<Tarea>(async (tarea) => await EditarTareaAsync(tarea));
            EliminarTareaCommand = new Command<Tarea>(async (tarea) => await EliminarTareaAsync(tarea));
            MarcarEntregadaCommand = new Command<Tarea>(async (tarea) => await MarcarEntregadaAsync(tarea));
        }

        public ObservableCollection<Tarea> Tareas { get; }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        public ICommand CargarTareasCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CrearTareaCommand { get; }
        public ICommand EditarTareaCommand { get; }
        public ICommand EliminarTareaCommand { get; }
        public ICommand MarcarEntregadaCommand { get; }

        // Se ejecuta cuando la página aparece
        public async Task InitializeAsync()
        {
            await CargarTareasAsync();
        }

        private async Task CargarTareasAsync()
        {

            System.Diagnostics.Debug.WriteLine($">>> Iniciando carga de tareas");
            System.Diagnostics.Debug.WriteLine($">>> IsAuthenticated: {_apiService.IsAuthenticated()}");

            if (IsBusy)
                return;

            IsBusy = true;
            MensajeError = string.Empty;

            try
            {
                // La ruta correcta es /tarea (sin /api/)
                var tareas = await _apiService.GetAsync<List<Tarea>>("tarea");

                Tareas.Clear();

                if (tareas != null && tareas.Any())
                {
                    foreach (var tarea in tareas)
                    {
                        Tareas.Add(tarea);
                    }
                }
                else
                {
                    MensajeError = "No hay tareas registradas";
                }
            }
            catch (HttpRequestException ex)
            {
                MensajeError = $"Error de conexión: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($">>> Error HTTP: {ex}");
                await Application.Current.MainPage.DisplayAlert(
                    "Error de Conexión",
                    "No se pudo conectar con el servidor. Verifica que la API esté corriendo.",
                    "OK");
            }
            catch (UnauthorizedAccessException)
            {
                MensajeError = "Sesión expirada. Inicia sesión nuevamente.";
                await Application.Current.MainPage.DisplayAlert(
                    "Sesión Expirada",
                    "Tu sesión ha expirado. Inicia sesión nuevamente.",
                    "OK");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                MensajeError = $"Error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($">>> Error completo: {ex}");
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    MensajeError,
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            await CargarTareasAsync();
            IsRefreshing = false;
        }

        private async Task CrearTareaAsync()
        {
            await Shell.Current.GoToAsync("CrearEditarTareaPage");
        }

        private async Task EditarTareaAsync(Tarea tarea)
        {
            if (tarea == null)
                return;

            // Verificar si la tarea está entregada
            if (tarea.Estatus)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "No Permitido",
                    "No se puede editar una tarea que ya fue entregada.",
                    "OK");
                return;
            }

            await Shell.Current.GoToAsync($"CrearEditarTareaPage?tareaId={tarea.Id}");
        }

        private async Task EliminarTareaAsync(Tarea tarea)
        {
            if (tarea == null)
                return;

            // Verificar si la tarea está entregada
            if (tarea.Estatus)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "No Permitido",
                    "No se puede eliminar una tarea que ya fue entregada.",
                    "OK");
                return;
            }

            var confirmar = await Application.Current.MainPage.DisplayAlert(
                "Confirmar Eliminación",
                $"¿Estás seguro de eliminar la tarea '{tarea.Titulo}'?",
                "Sí", "No");

            if (!confirmar)
                return;

            try
            {
                await _apiService.DeleteAsync<object>($"tarea/{tarea.Id}");

                Tareas.Remove(tarea);

                await Application.Current.MainPage.DisplayAlert(
                    "Éxito",
                    "Tarea eliminada correctamente",
                    "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"No se pudo eliminar la tarea: {ex.Message}",
                    "OK");
            }
        }

        private async Task MarcarEntregadaAsync(Tarea tarea)
        {
            if (tarea == null)
                return;

            // Si ya está entregada, no hacer nada
            if (tarea.Estatus)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Información",
                    "Esta tarea ya fue marcada como entregada.",
                    "OK");
                return;
            }

            var confirmar = await Application.Current.MainPage.DisplayAlert(
                "Confirmar Entrega",
                $"¿Marcar la tarea '{tarea.Titulo}' como entregada?\n\nNota: No podrás editarla ni eliminarla después.",
                "Sí, marcar como entregada", "Cancelar");

            if (!confirmar)
                return;

            try
            {
                // Actualizar el estatus a true
                tarea.Estatus = true;

                // Enviar actualización a la API
                await _apiService.PutAsync($"tarea/{tarea.Id}", tarea);

                // Actualizar la UI
                var tareaEnLista = Tareas.FirstOrDefault(t => t.Id == tarea.Id);
                if (tareaEnLista != null)
                {
                    var index = Tareas.IndexOf(tareaEnLista);
                    Tareas[index] = tarea;
                }

                await Application.Current.MainPage.DisplayAlert(
                    "Éxito",
                    "Tarea marcada como entregada correctamente.",
                    "OK");

                // Recargar la lista para actualizar la UI
                await CargarTareasAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"No se pudo marcar la tarea como entregada: {ex.Message}",
                    "OK");
            }
        }
    }
}
