using Grocery.App.ViewModels;
using Grocery.Core.Models;

namespace Grocery.App.Views;

public partial class BoughtProductsView : ContentPage
{
    private readonly BoughtProductsViewModel _viewModel;

    public BoughtProductsView(BoughtProductsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (sender is Picker picker && picker.SelectedItem is Product product)
        {
            _viewModel.NewSelectedProduct(product);
        }
    }
}
