using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using UserLoginInfo = Sras.PublicCoreflow.ConferenceManagement.UserLoginInfo;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
using Volo.Abp.Identity;
using Volo.Abp.Validation;
using Volo.Abp.Users;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Volo.Abp.Data;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("SrasUserAccount")]
    [Route("api/sras/account")]
    public class UserController : AbpController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IdentityUserManager _userManager;
        private readonly ICurrentUser _currentUser;
        private readonly IConfiguration _configuration;
        private readonly IUserAppService _userAppService;

        public UserController(SignInManager<IdentityUser> signInManager, IdentityUserManager userManager, ICurrentUser currentUser, IConfiguration configuration, IUserAppService userAppService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _currentUser = currentUser;
            _configuration = configuration;
            _userAppService = userAppService;
        }

        [HttpGet]
        [Route("logout")]
        public virtual async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        [HttpPost]
        [Route("checkPassword")]
        public virtual async Task<AbpLoginResult> CheckPassword(UserLoginInfo login)
        {
            ValidateLoginInfo(login);

            await ReplaceEmailToUsernameOfInputIfNeeds(login);

            var identityUser = await _userManager.FindByNameAsync(login.UserNameOrEmailAddress);

            if (identityUser == null)
            {
                return new AbpLoginResult(LoginResultType.InvalidUserNameOrPassword);
            }

            return GetAbpLoginResult(await _signInManager.CheckPasswordSignInAsync(identityUser, login.Password, true));
        }

        [HttpPost("login")]
        public async Task<object> Login(UserLoginInfo login)
        {
            ValidateLoginInfo(login);

            await ReplaceEmailToUsernameOfInputIfNeeds(login);

            var identityUser = await _userManager.FindByNameAsync(login.UserNameOrEmailAddress);

            if (identityUser == null)
            {
                return new AbpLoginResult(LoginResultType.InvalidUserNameOrPassword);
            }

            var result = GetAbpLoginResult(await _signInManager.CheckPasswordSignInAsync(identityUser, login.Password, true));

            if (result.Result != LoginResultType.Success)
            {
                return result;
            }

            return Ok(new
            {
                result = result,
                token = await GenerateTokenAsync(identityUser)
            });
        }

        [HttpGet("test")]
        [Authorize]
        public IActionResult TestLogin()
        {
            var id = _currentUser.Id;

            return Ok("Ok" + id);
        }

        private async Task<string> GenerateTokenAsync(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyBytes = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty);

            var roles = await _userAppService.GetRolesAsync(user.Id);

            var claims = new List<Claim> {
                                    new Claim(AbpClaimTypes.UserId, user.Id.ToString()),
                                    new Claim(ClaimTypes.Email, user.Email),
                                    new Claim(AbpClaimTypes.Name, user.Name),
                                    new Claim("middle_name", user.GetProperty<string?>(AccountConsts.MiddleNamePropertyName) ?? string.Empty),
                                    new Claim(AbpClaimTypes.SurName, user.Surname ?? string.Empty),
                                    new Claim("country", user.GetProperty<string?>(AccountConsts.CountryPropertyName) ?? string.Empty),
                                    new Claim("organization", user.GetProperty<string?>(AccountConsts.OrganizationPropertyName) ?? string.Empty),
                                    new Claim("token_id", Guid.NewGuid().ToString())
                                };

            roles.ForEach(x =>
            {
                claims.AddLast(new Claim(AbpClaimTypes.Role, x.Name));
            });

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);

            return jwtTokenHandler.WriteToken(token);
        }

        private static AbpLoginResult GetAbpLoginResult(SignInResult result)
        {
            if (result.IsLockedOut)
            {
                return new AbpLoginResult(LoginResultType.LockedOut);
            }

            if (result.IsNotAllowed)
            {
                return new AbpLoginResult(LoginResultType.NotAllowed);
            }

            if (!result.Succeeded)
            {
                return new AbpLoginResult(LoginResultType.InvalidUserNameOrPassword);
            }

            return new AbpLoginResult(LoginResultType.Success);
        }

        protected virtual async Task ReplaceEmailToUsernameOfInputIfNeeds(UserLoginInfo login)
        {
            if (!ValidationHelper.IsValidEmailAddress(login.UserNameOrEmailAddress))
            {
                return;
            }

            var userByUsername = await _userManager.FindByNameAsync(login.UserNameOrEmailAddress);
            if (userByUsername != null)
            {
                return;
            }

            var userByEmail = await _userManager.FindByEmailAsync(login.UserNameOrEmailAddress);
            if (userByEmail == null)
            {
                return;
            }

            login.UserNameOrEmailAddress = userByEmail.UserName;
        }

        private void ValidateLoginInfo(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentException(nameof(login));
            }

            if (login.UserNameOrEmailAddress.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(login.UserNameOrEmailAddress));
            }

            if (login.Password.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(login.Password));
            }
        }
    }
}
