using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProductsService.Controllers.Models.Requests;
using ProductsService.Infrastructure.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<StoreUser> _userManager;
        private readonly SignInManager<StoreUser> _signInManager;
        private readonly IConfiguration _config;

        public UsersController(
            UserManager<StoreUser> userManager,
            SignInManager<StoreUser> signInManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] SignInRequest request)
        {
            var user = new StoreUser { UserName = request.Email, Email = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                return Ok(new { Message = "User created successfully" });
            }

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Logins an user
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] SignInRequest request)
        {
            var result = await _signInManager
                .PasswordSignInAsync(request.Email, request.Password, false, false);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Logged in successfully" });
            }

            return Unauthorized();
        }

        /// <summary>
        /// Creates a JWT token which has to be provided to call secured endpoints. 
        /// To generate token you have to provide already registered user email and password
        /// </summary>
        [HttpPost("createToken")]
        public async Task<IActionResult> CreateToken([FromBody] SignInRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Email);

            if (user != null)
            {
                var result = await _signInManager
                    .CheckPasswordSignInAsync(user, request.Password, false);

                if (!result.Succeeded)
                {
                    return Unauthorized();
                }

                if (result.Succeeded)
                {
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Token:Key"]));
                    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        _config["Token:Issuer"],
                        _config["Token:Audience"],
                        claims,
                        signingCredentials: credentials,
                        expires: DateTime.Now.AddMinutes(20));

                    return Created("", new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }
            }

            return BadRequest();
        }
    }

}
