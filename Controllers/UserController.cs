using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IteraEmpresaGrupos.Models;
using IteraEmpresaGrupos.Services;

namespace IteraEmpresaGrupos.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly TokenService _tokenService;

        public UserController(UserService userService, TokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Login(LoginModel loginModel)
        {
            var user = await _userService.GetUserByNameAndPasswordAsync(loginModel.Name, loginModel.Password);

            if (user == null)
            {
                return Unauthorized("Nome de usuário ou senha incorretos");
            }

            var token = _tokenService.GenerateToken(user.Id);

            return new { user, token };
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<UserWithToken>> RefreshToken(TokenModel tokenModel)
        {
            var principal = _tokenService.DecodeToken(tokenModel.Token);
            var username = principal.Identity?.Name;

            var user = await _userService.GetUserByNameAsync(username);

            if (user == null)
            {
                return BadRequest("Invalid token");
            }

            var token = _tokenService.GenerateToken(user.Id);

            return new UserWithToken(user, token);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<User>> GetCurrentUser(TokenModel tokenModel)
        {
            var principal = _tokenService.DecodeToken(tokenModel.Token);
            var username = principal.Identity?.Name;
            var user = await _userService.GetUserByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> CreateUser(CreateUserModel userModel)
        {
            var user = new User
            {
                Name = userModel.Username,
                Password = userModel.Password,
            };

            await _userService.CreateUserAsync(user);

            return CreatedAtAction(nameof(GetCurrentUser), null, user);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserModel userModel)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.Name = userModel.Username;
            user.Password = userModel.Password;


            await _userService.UpdateUserAsync(user.Id, user);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            await _userService.RemoveUserAsync(id);

            return NoContent();
        }
    }
    public class UpdateUserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateUserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserWithToken
    {
        public User User { get; set; }
        public string Token { get; set; }

        public UserWithToken(User user, string token)
        {
            User = user;
            Token = token;
        }
    }

    public class TokenModel
    {
        public string Token { get; set; }
    }

    public class LoginModel
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }

}
