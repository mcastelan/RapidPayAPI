using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RapidPayAPI.Dtos;
using RapidPayAPI.Models;
using UserToken = RapidPayAPI.Dtos.UserToken;


namespace RapidPayAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
      

        public AuthenticationController(UserManager<User> userManager, 
        RoleManager<IdentityRole> roleManager,
         IConfiguration configuration)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._configuration = configuration;
           
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] AuthenticationRequestDto request)
        {

            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return BadRequest("Invalid login attempt");
            }

           
                
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(BuildToken(user, roles));
           
            
            

          
        }

        // private async Task<User> GetUser(string email, string password)
        // {
        //     return await _context.UserInfos.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        // }

        private UserToken BuildToken(User user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.GivenName, $"{user.NormalizedEmail} {user.NormalizedUserName}")
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
             var expiration = DateTime.UtcNow.AddMinutes(30);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);
            
            var jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return new UserToken()
            {
                Token = jwt,
                Expiration = expiration
            };
        }

       
    
    }
}
