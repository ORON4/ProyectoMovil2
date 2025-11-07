using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ProyectoMovil2.Services;
using ProyectoMovil2.Models;

namespace ProyectoMovil2.ViewModels
{
    [QueryProperty(nameof(TareaId), "tareaId")]
    public class CrearEditarTareaViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private int _tareaId;
        private string _titulo;
        private string _descripcion;
        private DateTime _fechaEntrega;
        private bool _estatus;
        private string _mensajeError;
        private bool _esEdicion;

        public CrearEditarTareaViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Nueva Tarea";

            // Valores por defecto
            FechaEntrega = DateTime.Now.AddDays(7);
            FechaMinima = DateTime.Now;

            GuardarCommand = new Command(async () => await GuardarAsync(), () => PuedeGuardar());
            CancelarCommand = new Command(async () => await CancelarAsync());
        }

        public int TareaId
        {
            get => _tareaId;
            set
            {
                _tareaId = value;
                if (value > 0)
                {
                    _esEdicion = true;
                    Title = "Editar Tarea";
                    Task.Run(async () => await CargarTareaAsync());
                }
            }
        }

        public string Titulo
        {
            get => _titulo;
            set
            {
                SetProperty(ref _titulo, value);
                ((Command)GuardarCommand).ChangeCanExecute();
            }
        }

        public string Descripcion
        {
            get => _descripcion;
            set => SetProperty(ref _descripcion, value);
        }

        public DateTime FechaEntrega
        {
            get => _fechaEntrega;
            set => SetProperty(ref _fechaEntrega, value);
        }

        public DateTime FechaMinima { get; }

        public bool Estatus
        {
            get => _estatus;
            set => SetProperty(ref _estatus, value);
        }

        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        public ICommand GuardarCommand { get; }
        public ICommand CancelarCommand { get; }

        private bool PuedeGuardar()
        {
            return !string.IsNullOrWhiteSpace(Titulo) && !IsBusy;
        }

        private async Task CargarTareaAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var tarea = await _apiService.GetAsync<Tarea>($"tarea/{TareaId}");

                if (tarea != null)
                {
                    Titulo = tarea.Titulo;
                    Descripcion = tarea.Descripcion;
                    FechaEntrega = tarea.FechaEntrega;
                    Estatus = tarea.Estatus;

                    // Si la tarea está entregada, mostrar mensaje y volver
                    if (tarea.Estatus)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            "No Permitido",
                            "No se puede editar una tarea que ya fue entregada.",
                            "OK");
                        await Shell.Current.GoToAsync("..");
                    }
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar la tarea: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task GuardarAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            MensajeError = string.Empty;

            try
            {
                var tarea = new Tarea
                {
                    Id = TareaId,
                    Titulo = Titulo.Trim(),
                    Descripcion = Descripcion?.Trim() ?? "",
                    FechaEntrega = FechaEntrega,
                    Estatus = Estatus
                };

                if (_esEdicion)
                {
                    // Actualizar tarea existente (PUT)
                    await _apiService.PutAsync($"tarea/{tarea.Id}", tarea);
                    await Application.Current.MainPage.DisplayAlert(
                        "Éxito",
                        "Tarea actualizada correctamente",
                        "OK");
                }
                else
                {
                    // Crear nueva tarea (POST)
                    await _apiService.PostAsync<Tarea, Tarea>("tarea", tarea);
                    await Application.Current.MainPage.DisplayAlert(
                        "Éxito",
                        "Tarea creada correctamente",
                        "OK");
                }

                // Volver a la página anterior
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al guardar: {ex.Message}";
                await Application.Current.MainPage.DisplayAlert("Error", MensajeError, "OK");
            }
            finally
            {
                IsBusy = false;
                ((Command)GuardarCommand).ChangeCanExecute();
            }
        }

        private async Task CancelarAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }


}
