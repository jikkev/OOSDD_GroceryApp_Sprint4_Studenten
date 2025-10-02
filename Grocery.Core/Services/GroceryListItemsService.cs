using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item);
        }

        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            // Haal alle GroceryListItems op (elke aankoop van een product)
            var allItems = _groceriesRepository.GetAll();

            // Groepeer per ProductId en tel het aantal verkopen (Amount optellen)
            var productSales = allItems
                .GroupBy(item => item.ProductId)
                .Select(group => new
                {
                    ProductId = group.Key,
                    NrOfSells = group.Sum(item => item.Amount)
                })
                .OrderByDescending(x => x.NrOfSells)
                .Take(topX)
                .ToList();

            // Haal alle producten op voor naam en voorraad
            var allProducts = _productRepository.GetAll().ToDictionary(p => p.Id);

            // Maak de lijst van BestSellingProducts
            var result = new List<BestSellingProducts>();
            int ranking = 1;
            foreach (var sale in productSales)
            {
                if (allProducts.TryGetValue(sale.ProductId, out var product))
                {
                    result.Add(new BestSellingProducts(
                        sale.ProductId,
                        product.name,
                        product.Stock,
                        sale.NrOfSells,
                        ranking++
                    ));
                }
            }

            return result;
        }

        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }
    }
}