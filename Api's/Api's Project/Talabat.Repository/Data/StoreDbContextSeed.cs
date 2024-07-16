using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;

namespace Talabat.Repository.Data
{
    public static class StoreDbContextSeed
    {

        public static async  Task SeedAsync(StoreDbContext _context)
        {


            // 1 - Brands


            if (_context.Brands.Count() == 0 )
            {


                // 1- Read Data From Json File

                var brandtData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/brands.json");


                // 2 - Convert Json String To The Needed Type

                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandtData);

                if (brands?.Count() > 0)
                {
                    foreach (var brand in brands)
                    {
                        _context.Set<ProductBrand>().Add(brand);
                    }

                    await _context.SaveChangesAsync();
                }


            }


            // 2 - Category 


            if (_context.Types.Count() == 0 )
            {



                // 1- Read Data From Json File

                var categoryData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/categories.json");


                // 2 - Convert Json String To The Needed Type

                var categories = JsonSerializer.Deserialize<List<ProductType>>(categoryData);

                if (categories?.Count() > 0)
                {
                    foreach (var category in categories)
                    {
                        _context.Set<ProductType>().Add(category);


                        await _context.SaveChangesAsync();

                    }




                }

            }


            // 3 - Products 


            if (_context.Products.Count() == 0 )
            {


                // 1- Read Data From Json File

                var productData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/products.json");


                // 2 - Convert Json String To The Needed Type

                var products = JsonSerializer.Deserialize<List<Product>>(productData);

                if (products?.Count() > 0)
                {
                    foreach (var product in products)
                    {
                        _context.Set<Product>().Add(product);


                        await _context.SaveChangesAsync();

                    }
                }

            }




            if (_context.Products.Count() == 0)
            {


                // 1- Read Data From Json File

                var deliveryData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/delivery.json");


                // 2 - Convert Json String To <List<deliveryMethod>>(deliveryData)

                var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryData);

                if (deliveryMethods?.Count() > 0)
                {
                    foreach (var deliveryMethod in deliveryMethods)
                    {
                        _context.DeliveryMethods.Add(deliveryMethod);
                    }
                        await _context.SaveChangesAsync();
                }

            }



        }   

    }
}
