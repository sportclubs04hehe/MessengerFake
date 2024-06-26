﻿using MessengerFake.API.Data;

namespace MessengerFake.API.Service.Impl
{
    public class UnitOfWork(DataContext _context, IUserRepository userRepository,
    IPhotoRepository photoRepository, IMessageRepository messageRepository, ILikesRepository likesRepository) : IUnitOfWork
    {

        public IUserRepository UserRepository => userRepository;

        public IPhotoRepository PhotoRepository => photoRepository;
        public IMessageRepository MessageRepository => messageRepository;

        public ILikesRepository LikesRepository => likesRepository;

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
