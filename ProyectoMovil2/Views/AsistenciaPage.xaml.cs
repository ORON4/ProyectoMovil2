using ProyectoMovil2.ViewModels;

namespace ProyectoMovil2.Views;

public partial class AsistenciaPage : ContentPage
{
    private readonly AsistenciaPageViewModel _viewModel;

    public AsistenciaPage(AsistenciaPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel; // Guarda la instancia
    }

    // Carga los datos cada vez que la página aparece
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarAlumnosAsync();
    }
}