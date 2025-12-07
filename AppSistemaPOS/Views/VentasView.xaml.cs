using AppSistemaPOS.ViewModels;

namespace AppSistemaPOS.Views;

public partial class VentasView : ContentPage
{
	public VentasView(VentasViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
}
