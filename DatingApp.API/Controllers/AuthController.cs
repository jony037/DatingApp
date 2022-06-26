using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
 
 [ApiController]
[Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
  public AuthController(IAuthRepository repo,IConfiguration config)
  {
        _repo=repo;
        _config=config;
  }
  [HttpPost("register")]
  public async Task<IActionResult> Register(UserForRegisterDto UserForRegisterDto )
  {

 UserForRegisterDto.Username=UserForRegisterDto.Username.ToLower();

  if(await _repo.UserExists(UserForRegisterDto.Username))
     return BadRequest("Username is already exist");
  
      var  userToCreate=new User
      {
        Name=UserForRegisterDto.Username
      };
  
  
    var createdUser=await _repo.Register(userToCreate,UserForRegisterDto.Password);
    return StatusCode(201);
        
    
  } 
    [HttpPost("login")]
      public async Task<IActionResult> Login(UserForLoginDto UserForLoginDto )
      {

         var userFromRepo=await _repo.Login(UserForLoginDto.Username,UserForLoginDto.Password);
           if(userFromRepo==null)
           return Unauthorized();

      
      var claims=new []
      {
        new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
        new Claim(ClaimTypes.Name,userFromRepo.Name)
      };


        var key=new SymmetricSecurityKey (Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));     
      
        var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
      
        var tokenDescriptor=new SecurityTokenDescriptor
       { 
        Subject=new ClaimsIdentity(claims),
        Expires=DateTime.Now.AddDays(1),
        SigningCredentials=creds
       };
     
       var tokenHandler=new JwtSecurityTokenHandler();

       var token=tokenHandler.CreateToken(tokenDescriptor);


        return Ok(new{
          token=tokenHandler.WriteToken(token)
        });
      } 
    }
}