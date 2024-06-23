using MessengerFake.API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MessengerFake.API.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, Guid,
        IdentityUserClaim<Guid>, AppUserRole, IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<UserLike> Likes => Set<UserLike>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<Connection> Connections => Set<Connection>();
        public DbSet<Photo> Photos => Set<Photo>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
           .HasMany(ur => ur.UserRoles)
           .WithOne(u => u.User)
           .HasForeignKey(ur => ur.UserId)
           .IsRequired();

            builder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

            #region Giải thích
            /**
             tạo ra khóa chính (primary key) từ hai thuộc tính SourceUserId và TargetUserId. 
            Điều này đảm bảo rằng mỗi cặp người dùng (người gửi lượt thích và người nhận lượt thích)
            là duy nhất trong cơ sở dữ liệu, Bạn không thể có hai lượt thích với cùng một SourceUserId và TargetUserId*/
            #endregion
            builder.Entity<UserLike>()
            .HasKey(k => new { k.SourceUserId, k.TargetUserId });

            #region Giải thích
            /*
             * Mối quan hệ một-nhiều: Một AppUser có thể gửi nhiều lượt thích (UserLike) và cũng có thể nhận nhiều lượt thích từ các người dùng khác.
            Khóa ngoại: UserLike có hai khóa ngoại SourceUserId và TargetUserId để liên kết với AppUser.
            Cascade Delete: Khi một AppUser bị xóa, tất cả các lượt thích (UserLike) mà liên quan tới người dùng này sẽ bị xóa theo. Điều này đảm bảo tính toàn vẹn của dữ liệu.**/
            #endregion
            builder.Entity<UserLike>()
            .HasOne(s => s.SourceUser)
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(s => s.SourceUserId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<UserLike>()
            .HasOne(s => s.TargetUser)
            .WithMany(l => l.LikedByUsers)
            .HasForeignKey(s => s.TargetUserId)
            .OnDelete(DeleteBehavior.NoAction);

            #region Giải thích
            /*Mỗi người dùng (AppUser) có thể nhận nhiều tin nhắn (Message)
             * DeleteBehavior.Restrict => Khi một AppUser (người nhận) bị xóa, các tin nhắn mà người dùng này nhận sẽ không bị xóa.
             * Mối quan hệ một-nhiều: Một người dùng (AppUser) có thể gửi nhiều tin nhắn (Message). Mỗi tin nhắn có một người gửi.
             */
            #endregion
            builder.Entity<Message>()
              .HasOne(u => u.Recipient)
              .WithMany(m => m.MessagesReceived)
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
            .HasOne(u => u.Sender)
            .WithMany(m => m.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
