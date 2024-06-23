using MessengerFake.API.Extensions;
using MessengerFake.API.Service;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MessengerFake.API.Helpers
{
    #region Giải thích
    /*
     IAsyncActionFilter là một phần của ASP.NET Core Middleware cho phép bạn chèn logic vào 
    trước và sau khi một hành động trong controller được thực hiện.

     */
    #endregion
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Thực thi logic trước khi hành động trong controller được gọi
            //-----------------------------------------------------//
            // Thực thi hành động trong controller
            var resultContext = await next();

            // Kiểm tra xem người dùng có được xác thực hay không
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            // Lấy userId từ ClaimsPrincipal
            var userId = resultContext.HttpContext.User.GetUserId();

            // Lấy IUnitOfWork từ dịch vụ yêu cầu
            var uow = resultContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();

            // Lấy thông tin người dùng từ UserRepository
            var user = await uow.UserRepository.GetUserByIdAsync(userId);

            // Cập nhật LastActive của người dùng
            user.LastActive = DateTime.UtcNow;

            // Lưu các thay đổi vào cơ sở dữ liệu
            await uow.Complete();
        }
    }
}
