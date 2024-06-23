using AutoMapper;
using MessengerFake.API.Extensions;
using MessengerFake.API.Models.DTOs;
using MessengerFake.API.Models.Entities;

namespace MessengerFake.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            #region Giải thích
            /*
             * ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url)):
                dest.PhotoUrl: Trường đích PhotoUrl trong MemberDto.
                src.Photos.FirstOrDefault(x => x.IsMain).Url: Lấy URL của ảnh chính (IsMain) từ danh sách ảnh của AppUser 
            cấu hình AutoMapper giúp đảm bảo rằng tất cả các giá trị thời gian được ánh xạ sẽ có múi giờ nhất
            quán là UTC. Điều này giúp tránh các lỗi liên quan đến múi giờ và đảm bảo tính nhất quán khi lưu trữ,
            xử lý, và truyền dữ liệu thời gian trong ứng dụng của bạn.
             **/
            #endregion
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.PhotoUrl, opt =>
                    opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<Photo, PhotoDto>();

            CreateMap<MemberUpdateDto, AppUser>();

            CreateMap<RegisterDto, AppUser>();

            CreateMap<Message, MessageDto>()
                .ForMember(d => d.SenderPhotoUrl, o => o.MapFrom(s => s.Sender.Photos
                    .FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.RecipientPhotoUrl, o => o.MapFrom(s => s.Recipient.Photos
                    .FirstOrDefault(x => x.IsMain).Url));

            CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));

            CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue ?
                DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);

        }
    }
}
