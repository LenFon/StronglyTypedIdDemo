using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StronglyTypedIdDemo.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StronglyTypedIdDemo.Controllers
{
    /// <summary>
    /// 订单
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly StronglyTypedIdDemoDbContext _db;
        public OrdersController(ILogger<OrdersController> logger, StronglyTypedIdDemoDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        /// <summary>
        /// 获取所有订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Order>> Get()
        {
            return await _db.Orders.ToListAsync();
        }

        /// <summary>
        /// 根据Id获取指定的订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<Order> Get(OrderId id)
        {
            return await _db.Orders.FirstOrDefaultAsync(w => w.Id == id);
        }

        /// <summary>
        /// 添加订单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Order> Post(Order order)
        {
            var o = await _db.Orders.FirstOrDefaultAsync(w => w.Id == order.Id);

            if (o == null)
            {
                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                o = order;
            }

            return o;
        }
    }
}
