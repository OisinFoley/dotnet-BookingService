using BookingService.Data.Abstract;
using BookingService.Models;
using System.Collections.Generic;
using System.Linq;

namespace BookingService.Data.Concrete
{
    public sealed class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        public MessageRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        IEnumerable<Message> IMessageRepository.Messages => GetDbSet().AsEnumerable();
    }
}
