using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager; 
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            //retrieve user based on email
            var user = _db.ApplicationUsers.FirstOrDefault(u=>u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if(!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                //assign role to User
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            //when login request comes we first needs to fetch that user from Db
            var user = _db.ApplicationUsers.FirstOrDefault(u=>u.UserName.ToLower() == loginRequestDto.UserName.ToLower());
            bool IsValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            //invalid User
            if (user == null || IsValid == false)
            {
                return new LoginResponseDto() { User = null, Token = "" };
            }

            //get all roles of user
            var roles = await _userManager.GetRolesAsync(user);

            //if user is found, generate JWT Token
            var token = _jwtTokenGenerator.GenerateToken(user, roles);


            //userDto is generated for clients to see their credentials typically say dashboards
            UserDto userDto = new()
            {
                Email = user.Email,
                Name = user.Name,
                Id = user.Id,
                PhoneNumber = user.PhoneNumber
            };

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDto,
                Token = token
            };
            return loginResponseDto;
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            /*A new instance of ApplicationUser is created using the data provided in RegistrationRequestDto.
                This object represents the user to be registered.
            ApplicationUser: This entity might contain various properties and fields managed by the Identity framework,
            such as PasswordHash, SecurityStamp, ConcurrencyStamp, TwoFactorEnabled, and more.
            Exposing these fields directly could pose security risks and result in sending unnecessary data.*/


            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };
            try
            {
                //method to create a user in the Database
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    //After user creation, the code fetches the user from the database
                    var userToReturn = _db.ApplicationUsers.First(u=>u.UserName == registrationRequestDto.Email);


                    /*because of identity in .net there are multiple identity columns that application user has.
                    out of that only i want to expose the columns that the registrationrequest has,
                    so we created a userdto*/

                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        Name = userToReturn.Name,
                       Id = userToReturn.Id,
                       PhoneNumber = userToReturn.PhoneNumber
                    };
                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {

            }
            return "Error Encountered";     
        }
    }
}
