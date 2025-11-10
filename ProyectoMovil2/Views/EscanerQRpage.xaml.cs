using ProyectoMovil2.ViewModels;
using ZXing.Net.Maui;   




namespace ProyectoMovil2.Views;

public partial class EscanerQRpage : ContentPage
{
    private readonly EscanerQRpageViewModel _viewModel;

    public EscanerQRpage(EscanerQRpageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    private void cameraBarcodeReaderView_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        // Este evento se dispara desde el control ZXing
        // y llama al Comando en nuestro ViewModel
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (e.Results.Any())
            {
                _viewModel.DetectarCodigoCommand.Execute(e.Results[0].Value);
            }
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Inicia la cámara cuando la página aparece
        cameraBarcodeReaderView.IsDetecting = true;
    }

    protected override void OnDisappearing()
    {
        // Detiene la cámara cuando la página desaparece
        cameraBarcodeReaderView.IsDetecting = false;
        base.OnDisappearing();
    }
}