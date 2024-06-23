
using AutoMapper;
using MessengerFake.API.Data;

namespace MessengerFake.API.Service.Impl
{
    public class UnitOfWork(DataContext _context, IUserRepository userRepository,
    IPhotoRepository photoRepository) : IUnitOfWork
    {

        public IUserRepository UserRepository => userRepository;

        public IPhotoRepository PhotoRepository => photoRepository;

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
