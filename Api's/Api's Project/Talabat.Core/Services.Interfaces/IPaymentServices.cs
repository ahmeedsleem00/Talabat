using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;

namespace Talabat.Core.Services.Interfaces
{
    public interface IPaymentServices
    {
        Task<CustomerBasket?> CreateOrUpdatePaymentEntity(string basketId);

        Task<Order> UpdatePaymentIntintToSuccessedOrFailed(string paymentEntintId, bool flag);
    }
}
