using Portfolio.BackEnd.Common.Models;

namespace Portfolio.BackEnd.Dal.Auth
{
    public interface IAuthManagerService
    {
        Task<object> Login(LoginModel model);
        Task<Response> Register(RegisterModel model);
        Task<Response> RegisterAdmin(RegisterModel model);
    }
}