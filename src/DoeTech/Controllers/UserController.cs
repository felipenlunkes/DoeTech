using DoeTech.DTOs;
using DoeTech.Models;
using DoeTech.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace DoeTech.Controllers;

[Route("rest/v{version:apiVersion}/user")]
[ApiVersion("1")] // Define a versão da API
public class AuthController : ControllerBase
{
    private readonly UserLoginService _loginService;

    public AuthController(UserLoginService loginService)
    {
        _loginService = loginService;
    }
    
    [HttpPost]
    public IActionResult Add([FromBody] User request)
    {
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var user = _loginService.AddUser(request);
        
        return Ok(user);
    }
    
    [Authorize]
    [HttpPut("{userId}")]
    public IActionResult Update(Guid userId, [FromBody] User request)
    {
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var user = _loginService.Update(userId, request);
        
        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{userId}")]
    public IActionResult Delete(Guid userId)
    {
        _loginService.RemoveUser(userId);
        
        return NoContent();
    }
    
    [Authorize]
    [HttpGet("{userId}")]
    public IActionResult Get(Guid userId)
    {
        var user = _loginService.GetById(userId);
        
        return Ok(user);
    }
      
    [Authorize]
    [HttpGet("query")]
    public IActionResult Query([FromQuery] UserQueryDto filter)
    {
        var results = _loginService.Query(filter);
        
        return Ok(results);
    }
    
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        return _loginService.Login(request);
    }
    
    [HttpPost("recover")]
    public IActionResult RecoverPassword([FromBody] RecoverPasswordDto request)
    {
        
        _loginService.RecoverPassword(request);
        
        return Ok();
    }
}