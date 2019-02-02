using BookingService.Models;
using System.Collections.Generic;

namespace BookingService.Data.Abstract
{
    public interface IMessageRepository : IBaseRepository<Message>
    {
        IEnumerable<Message> Messages { get; }
    }
}
