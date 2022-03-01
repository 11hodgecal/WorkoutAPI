using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web.Http.ModelBinding;
using WorkoutAPI.Classes;
using WorkoutAPI.Data;
using WorkoutAPI.Models;

namespace WorkoutAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<UserModel> signInManager;
        private readonly UserManager<UserModel> userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        private readonly JwtBearerTokenSettings jwtBearerTokenSettings;
       

        public AuthController(IOptions<JwtBearerTokenSettings> jwtTokenOptions, UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            this.jwtBearerTokenSettings = jwtTokenOptions.Value;
            this.userManager = userManager;
            this._roleManager = roleManager;
            this._context = context;
        }

        

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel userDetails)
        {
            
            if (!ModelState.IsValid || userDetails == null)
            {
                return new BadRequestObjectResult(new { Message = "User Registration Failed" });
            }

            var identityUser = new UserModel() { UserName = userDetails.Email, Email = userDetails.Email, FirstName = userDetails.FirstName, LastName = userDetails.LastName  };
            //makes the account a normal user
            identityUser.Role = "user";
            var result = await userManager.CreateAsync(identityUser, userDetails.Password);
            //adds possible days to workout
            var days = new List<string>() { "Monday","Tuesday","Wednesday","Thursday","Friday","Saturday","Sunday" };
            foreach (var day in days)
            {
                var Workout = new WorkoutModel() {Name = day, Userid = identityUser.Id};
                await _context.Workouts.AddAsync(Workout);
            }
            
            if (!result.Succeeded)
            {
                var dictionary = new ModelStateDictionary();
                foreach (IdentityError error in result.Errors)
                {
                    dictionary.AddModelError(error.Code, error.Description);
                }

                return new BadRequestObjectResult(new { Message = "User Registration Failed", Errors = dictionary });
            }
            //saves the changes to the db 
            await _context.SaveChangesAsync();
            return Ok(new { Message = "User Reigstration Successful" });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] AuthenticateRequestModel credentials)
        {

            //https://thecodeblogger.com/2020/01/25/securing-net-core-3-api-with-cookie-authentication/
            UserModel identityUser;

            if (!ModelState.IsValid
                || credentials == null
                || (identityUser = await ValidateUser(credentials)) == null)
            {
                return new BadRequestObjectResult(new { Message = "Login failed" });
            }

            var token = GenerateToken(identityUser);
           


            return Ok(new AuthenticateResponseModel(identityUser, token.ToString()));
           
        }

        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok(new { Message = "You are logged out" });
        }





        private async Task<UserModel> ValidateUser(AuthenticateRequestModel credentials)
        {
            var identityUser = await userManager.FindByNameAsync(credentials.Email);
            if (identityUser != null)
            {
                var result = userManager.PasswordHasher.VerifyHashedPassword(identityUser, identityUser.PasswordHash, credentials.Password);
                return (UserModel)(result == PasswordVerificationResult.Failed ? null : identityUser);
            }

            return null;
        }


        private object GenerateToken(UserModel identityUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtBearerTokenSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Subject = new ClaimsIdentity(new Claim[]
                //{
                //    new Claim(ClaimTypes.Name, identityUser.UserName.ToString()),
                //    new Claim(ClaimTypes.Email, identityUser.Email)
                //}),

                Expires = DateTime.UtcNow.AddSeconds(jwtBearerTokenSettings.ExpiryTimeInSeconds),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = jwtBearerTokenSettings.Audience,
                Issuer = jwtBearerTokenSettings.Issuer
            };
            Console.WriteLine("Hello");
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}

