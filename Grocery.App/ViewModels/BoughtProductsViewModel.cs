using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    public partial class BoughtProductsViewModel : BaseViewModel
    {
        private readonly IBoughtProductsService _boughtProductsService;

        [ObservableProperty]
        private Product selectedProduct;

        public ObservableCollection<BoughtProducts> BoughtProductsList { get; set; } = [];
        public ObservableCollection<Product> Products { get; set; }

        [ObservableProperty]
        private string infoMessage;

        public BoughtProductsViewModel(IBoughtProductsService boughtProductsService, IProductService productService)
        {
            _boughtProductsService = boughtProductsService;
            Products = new(productService.GetAll());
            InfoMessage = "Selecteer eerst een product.";
        }

        partial void OnSelectedProductChanged(Product value)
        {
            BoughtProductsList.Clear();

            if (value == null)
            {
                InfoMessage = "Selecteer eerst een product.";
                return;
            }

            var results = _boughtProductsService.Get(value.Id);

            if (results == null || results.Count == 0)
            {
                InfoMessage = $"Geen klanten gevonden voor product '{value.Name}'.";
                return;
            }

            foreach (BoughtProducts boughtProduct in results)
            {
                BoughtProductsList.Add(boughtProduct);
            }

            InfoMessage = string.Empty;
        }

        [RelayCommand]
        public void NewSelectedProduct(Product product)
        {
            SelectedProduct = product;
        }
    }
}
