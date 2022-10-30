using Authentication_and_Authorization.Models.DTO;

namespace Authentication_and_Authorization.Repositories.Abstract
{
    public interface IUserAuthenticationService
    {
        Task<Status> LoginAsync(LoginModel model);
        Task<Status> RegistrationAsync(RegistrationModel model);
        Task<Status> ChangePasswordAsync(ChangePasswordModel model, string username);
        Task LogoutAsync();
    }
}
