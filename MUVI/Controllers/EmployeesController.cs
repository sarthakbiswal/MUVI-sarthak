using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MUVI.Data;
using MUVI.Model;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MUVI.Controllers
{
    [Route("api/v1/employees")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeDataLayer _edl;
        private readonly IConfiguration _configuration;

        public EmployeesController(EmployeeDataLayer edl, IConfiguration configuration)
        {
            _edl = edl;
            _configuration = configuration;

        }
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(User user)
        {
            var validUser = ValidateUser(user.UserName, user.Password);
            if (validUser != null)
            {
                var token = GenerateJwtToken(validUser);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }

        private User ValidateUser(string username, string password)
        { 
            if (username == "sarthak" && password == "sarthak123")
            {
                return new User { UserName = username };
            }

            return null;
        }

        private string GenerateJwtToken(User user)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
                var issuer = jwtSettings["Issuer"];
                var audience = jwtSettings["Audience"];

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                    Expires = DateTime.UtcNow.AddHours(5),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpGet]
        public IActionResult GetEmployees([FromQuery] int page_limit = 10, [FromQuery] int page_index = 1, [FromQuery] string search_text = "")
        {

            var paginatedData = _edl.SearchEmployees(page_limit, page_index, search_text);

            return Ok(paginatedData);
        }

        [HttpGet("{id}")]
        public IActionResult GetEmployee(int id)
        {
            var employee = _edl.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult PostEmployee(Employee employee)
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _edl.CreateEmployee(employee);

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        [HttpPut("{id}")]
        public IActionResult PutEmployee(int id, Employee employee)
        {
            if (id != employee.Id || User.Identity.Name != employee.Email)
            {
                return Forbid();
            }

            _edl.UpdateEmployee(employee);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteEmployee(int id)
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var employee = _edl.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }

            _edl.DeleteEmployee(id);

            return NoContent();
        }


    }
}

