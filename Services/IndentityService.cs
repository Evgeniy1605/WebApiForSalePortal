using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SalePortal.Data;
using SalePortal.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApiForSalePortal.Models;

namespace WebApiForSalePortal.Services
{
    public class IndentityService : IIndentityService
    {
        private readonly SalePortalDbConnection _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public IndentityService(SalePortalDbConnection context, IMapper mapper,
            IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }
        private string ToHashPassword(string password)
        {
            var sha = SHA256.Create();
            var asBiteArray = Encoding.Default.GetBytes(password);
            var hash = sha.ComputeHash(asBiteArray);
            return Convert.ToBase64String(hash);
        }
        public async ValueTask<UserOutPutModel> ValidateUserAsync(string userName, string password)
        {
            var admin = await _context.admins.SingleOrDefaultAsync(x => x.Name == userName 
            && x.Password == ToHashPassword(password));
            if (admin != null) 
            { 
                var resultAdmin = _mapper.Map<UserOutPutModel>(admin);
                string tokenForAdmin = CreatTockenForAdmin(resultAdmin);
                resultAdmin.Token = tokenForAdmin;
                resultAdmin.Role = "Admin";
                return resultAdmin;
            }
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Name == userName 
            && x.Password == ToHashPassword(password));
            if (user == null)
            {
                return new UserOutPutModel();
            }

            UserOutPutModel result = _mapper.Map<UserOutPutModel>(user);
            result.Role = "User";
            result.Token = CreatTocken(result);
            return result;
        }

        private string CreatTocken(UserOutPutModel user)
        {
            if (user is null) { return string.Empty; }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Surname, user.SurName),
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim(ClaimTypes.HomePhone, user.PhoneNumber),
                new Claim(ClaimTypes.Role, user.Role)
            };
            //
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("TokenKey").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(360),
                signingCredentials: creds,
                issuer: "WebApiForSalePortal",
                audience: "SalePortal"
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        private string CreatTockenForAdmin(UserOutPutModel user)
        {
            var claimsForAdmin = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, "Admin")
                };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("TokenKey").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claimsForAdmin,
                expires: DateTime.Now.AddDays(360),
                signingCredentials: creds,
                issuer: "WebApiForSalePortal",
                audience: "SalePortal"
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
