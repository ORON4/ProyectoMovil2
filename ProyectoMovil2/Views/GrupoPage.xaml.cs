using ProyectoMovil2.ViewModels;

namespace ProyectoMovil2.Views;

public partial class GrupoPage : ContentPage
{
	public GrupoPage(GrupoViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}