using AppSistemaPOS.ViewModels;

namespace AppSistemaPOS.Views;

public partial class EntradaInventarioView : ContentPage
{
	public EntradaInventarioView(EntradaInventarioViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
}
