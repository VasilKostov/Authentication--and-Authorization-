using Authentication_and_Authorization.Models.Domain;
using Authentication_and_Authorization.Models.DTO;
using Authentication_and_Authorization.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Authentication_and_Authorization.Repositories.Implementation
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserAuthenticationService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<Status> ChangePasswordAsync(ChangePasswordModel model, string username)
        {
            var status = new Status();

            var user =await userManager.FindByNameAsync(username);
            if (user == null)
            {
                status.StatusMessage = "User does not exist";
                status.StatusCode = 0;
                return status;
            }
            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassoword, model.NewPassword);
            if (result.Succeeded)
            {
                status.StatusMessage = "Password has updated successfully";
                status.StatusCode = 1;
            }
            else
            {
                status.StatusMessage = "Some error occcured";
                status.StatusCode = 0;
            }
            return status;
        }

        public async Task<Status> LoginAsync(LoginModel model)
        {
            var status = new Status();
            var user = await userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "Invalid username";
                return status;
            }

            //Match password
            if (!await userManager.CheckPasswordAsync(user,model.Password))
            {
                status.StatusCode = 0;
                status.StatusMessage = "Invalid password";
                return status;
            }

            var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, false, true);
            if (signInResult.Succeeded)
            {
                //Add Roles
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName)
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                status.StatusCode = 1;
                status.StatusMessage = "Logged in successfully";
                return status;
            }
            else if (signInResult.IsLockedOut)
            {
                status.StatusCode = 0;
                status.StatusMessage = "User is locked out";
                return status;
            }
            else
            {
                status.StatusCode = 0;
                status.StatusMessage = "Error on logging in";
                return status;
            }
        }

        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<Status> RegistrationAsync(RegistrationModel model)
        {
            var status = new Status();
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "User already exists";
                return status;
            }
            ApplicationUser user = new ApplicationUser
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Username,
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(user,model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode=0;
                status.StatusMessage = "User creation failed";
                return status;
            }

            //Role Managment
            if (!await roleManager.RoleExistsAsync(model.Role))
            {
                await roleManager.CreateAsync(new IdentityRole(model.Role));
            }
            if (await roleManager.RoleExistsAsync(model.Role))
            {
                await userManager.AddToRoleAsync(user,model.Role);
            }

            status.StatusCode = 1;
            status.StatusMessage = "User has registered successfully";
            return status;
        }
    }
}
