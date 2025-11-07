using ProyectoMovil2.ViewModels;

namespace ProyectoMovil2.Views;


public partial class AsistenciaPage : ContentPage
{
    private readonly AsistenciaPageViewModel _viewModel;
    public AsistenciaPage(AsistenciaPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Carga los datos cada vez que la página aparece
        await _viewModel.InitializeAsync();
    }
}