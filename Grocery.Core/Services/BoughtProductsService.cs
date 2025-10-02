
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;
        public BoughtProductsService(IGroceryListItemsRepository groceryListItemsRepository, IGroceryListRepository groceryListRepository, IClientRepository clientRepository, IProductRepository productRepository)
        {
            _groceryListItemsRepository=groceryListItemsRepository;
            _groceryListRepository=groceryListRepository;
            _clientRepository=clientRepository;
            _productRepository=productRepository;
        }
        public List<BoughtProducts> Get(int productId)
        {
            List<GroceryListItem> groceryListItems = _groceryListItemsRepository.GetAll().Where(g => g.ProductId == productId).ToList();
            List<BoughtProducts> boughtProducts = new List<BoughtProducts>();

            foreach (GroceryListItem item in groceryListItems)
            {
                GroceryList groceryList = _groceryListRepository.Get(item.GroceryListId);
                Client client = _clientRepository.Get(groceryList.ClientId);
                Product product = _productRepository.Get(item.ProductId);
                boughtProducts.Add(new BoughtProducts(client, groceryList, product));
            }

            return boughtProducts;
        }
    }
}
