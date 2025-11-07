using ProyectoMovil2.ViewModels;

namespace ProyectoMovil2.Views;

public partial class ReporteAsistenciaPage : ContentPage
{
    private readonly ReporteAsistenciaPageViewModel _viewModel;

    public ReporteAsistenciaPage(ReporteAsistenciaPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    // Carga los datos la primera vez que la página aparece
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarReporteAsync();
    }
}