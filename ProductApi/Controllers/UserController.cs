using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ProductApi.Models;
using System.Text;
using ProductApi.Dtos;
using ProductApi.Constants;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.AspNetCore.Authorization;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;

        public UserController(IConfiguration config)
        {
            this._config = config;
        }

        [HttpPost("sign-in", Name = "SignIn")]
        public ActionResult SingIn([FromBody] UserRequestDto userDto) {
            //if(!user.Email.Equals("camm@gmail.com") || !user.Password.Equals("camm"))
            //{
            //    return BadRequest();
            //}
            //var secretKeyBytes = Encoding.ASCII.GetBytes(secretKey);
            //var claims = new ClaimsIdentity();

            //claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Email));

            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = claims,
            //    Issuer = user.Email,
            //    Expires = DateTime.UtcNow.AddMinutes(5),
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            //};


            //var tokenHandler = new JwtSecurityTokenHandler();
            //var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            //string token = tokenHandler.WriteToken(tokenConfig);

            var user = UserList.Users.FirstOrDefault(userInList => userInList.Email.Equals(userDto.Email) && userInList.Password.Equals(userDto.Password));

            if (user == null) return NotFound(new { message = "User not Found" });

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["JwtSettings:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // Crear los claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Role, user.Rol),
                new Claim("test", "hola")
            };


            // Crear el token

            var tokenConfig = new JwtSecurityToken(
                _config["JwtSettings:Issuer"],
                _config["JwtSettings:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);

            string token = new JwtSecurityTokenHandler().WriteToken(tokenConfig);

            return Ok(new { token = token });
        }

        [HttpGet("get-userinfo", Name = "GetUserLogged")]
        //[Authorize]
        public ActionResult GetUserLogged()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity == null) return BadRequest();

                var userClaims = identity.Claims;

                User user = new User()
                {
                    Email = userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value,
                    FirstName = userClaims.FirstOrDefault(claim => claim.Type.Equals("test")).Value,

                };

                return Ok(user);
            }
            catch(Exception ex)
            {
                return BadRequest("No se envio ningun token");
            }
            
        }


    }
}
