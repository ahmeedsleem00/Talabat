using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Services.Interfaces;
using Talabat.Core.Specifications.Order_Specs;

namespace Talabat.Service
{
    public class OrderServices : IOrderServices
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentServices _paymentServices;

        public OrderServices(
            IBasketRepository basketRepository,
            IUnitOfWork unitOfWork,
            IPaymentServices paymentServices
            )
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _paymentServices = paymentServices;
        }

        public async Task<Order?> CreateOrderAsync(string BueyrEmail, string basketId, int DeliveryMethodId, Address ShippingAddress)
        {
            //1. Get Basket From Basket Repo
            var basket = await _basketRepository.GetBasketAsync(basketId);

            //2. Get Selected Items From Basket 
            var OrderItem = new List<OrderItem>();
            
            if(basket?.Items.Count() > 0)
            {
                foreach(var item in basket.Items)
                {
                    var product = await  _unitOfWork.Repository<Product>().GetAsync(item.Id);
                    var ProductItemOrderd = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
                    var orderItem = new OrderItem(ProductItemOrderd, item.Price, item.Quantity);

                    OrderItem.Add(orderItem);
                }
            }


            // 3. Calculate SubTotal

           var subTotal = OrderItem.Sum(OI => OI.Price * OI.Quantity);

            // 4. Get Delivery Method From Database 

             var deliveryMethod = await  _unitOfWork.Repository<DeliveryMethod>().GetAsync(DeliveryMethodId);

            //5. Create Order 

            //Check If Payment Intent Id Exists From Another Order 

            var spec = new OrderWithPaymentIntentSpecifications(basket.PaymentIntentId);
            var ExOrder = await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
            if(ExOrder is not null)
            {
                _unitOfWork.Repository<Order>().Delete(ExOrder);
                //Update Payment Intent Id With Amount Of Basket If Changed
             basket = await _paymentServices.CreateOrUpdatePaymentEntity(basketId);
            }


            var order = new Order(BueyrEmail, ShippingAddress, deliveryMethod, OrderItem, subTotal, basket.PaymentIntentId);

            //6. Add Order Local

           await _unitOfWork.Repository<Order>().AddAsync(order);

            // 7. Save Order in Database[]
            var result = await _unitOfWork.CompleteAsync(); 

            if (result <= 0) return null;

            return order;

        }

        public Task<IReadOnlyList<Order>?> GetOrdersForSpecificUserAsync(string BuyerEmail)
        {
            var spec = new OrderSpecifications(BuyerEmail);

           var orders = _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);

            return orders;
        }

        public async Task<Order?> GetOrderByIdForSpecificUserAsync(string BuyerEmail, int OrderId)
        {
            var spec = new OrderSpecifications(BuyerEmail, OrderId);

           

            var order = await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);


            if(order is null) return null;
            return order;
        } 


    }
}
