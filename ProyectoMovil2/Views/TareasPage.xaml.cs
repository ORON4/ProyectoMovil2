using ProyectoMovil2.ViewModels;

namespace ProyectoMovil2.Views;

public partial class TareasPage : ContentPage
{
    private readonly TareasViewModel _viewModel;
    public TareasPage(TareasViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
        BindingContext = viewModel;
	}

    protected override async void OnAppearing()
	{
        base.OnAppearing();

        // Carga las tareas cuando la página aparece
        await _viewModel.InitializeAsync();
    }

   
}