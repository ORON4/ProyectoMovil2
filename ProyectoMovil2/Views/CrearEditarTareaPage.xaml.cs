using ProyectoMovil2.ViewModels;

namespace ProyectoMovil2.Views;
 
public partial class CrearEditarTareaPage : ContentPage
{
	public CrearEditarTareaPage(CrearEditarTareaViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}