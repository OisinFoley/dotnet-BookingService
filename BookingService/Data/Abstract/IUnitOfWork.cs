using System.Threading.Tasks;

namespace BookingService.Data.Abstract
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}
