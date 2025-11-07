using ProyectoMovil2.ViewModels;

namespace ProyectoMovil2.Views;


public partial class AsistenciaPage : ContentPage
{
	public AsistenciaPage(AsistenciaPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}