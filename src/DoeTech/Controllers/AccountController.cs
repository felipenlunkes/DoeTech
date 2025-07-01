using DoeTech.DTOs;
using DoeTech.Models;
using DoeTech.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoeTech.Controllers;

[Route("rest/v{version:apiVersion}/account")]
[ApiVersion("1")] // Define a versão da API
public class AccountController : ControllerBase
{
    
    private readonly AccountService _accountService;
    private readonly AccountActivationService _accountActivationService;

    public AccountController(AccountService accountService, AccountActivationService accountActivationService)
    {
        _accountService = accountService;
        _accountActivationService = accountActivationService;
    }

    [HttpPost]
    public IActionResult Add([FromBody] Account request)
    {
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var account = _accountService.AddAccount(request);
        
        return Ok(account);
    }
    
    [Authorize]
    [HttpPut("{accountId}")]
    public IActionResult Update(Guid accountId, [FromBody] Account request)
    {
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var user = _accountService.UpdateAccount(accountId, request);
        
        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{accountId}")]
    public IActionResult Delete(Guid accountId)
    {
        _accountService.RemoveAccount(accountId);
        
        return NoContent();
    }
    
    [Authorize]
    [HttpGet("{accountId}")]
    public IActionResult GetById(Guid accountId)
    {
        var account = _accountService.GetById(accountId);
        
        return Ok(account);
    }
    
    [Authorize]
    [HttpGet("user/{userId}")]
    public IActionResult GetUserId(Guid userId)
    {
        var account = _accountService.GetByUserId(userId);
        
        return Ok(account);
    }
    
    [Authorize]
    [HttpGet("query")]
    public IActionResult Query([FromQuery] AccountQueryDto filter)
    {
        var results = _accountService.Query(filter);
        
        return Ok(results);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("{accountId}/activate")]
    public IActionResult Activate(Guid accountId)
    {
        _accountActivationService.ActivateAccount(accountId);
        
        return NoContent();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("{accountId}/deactivate")]
    public IActionResult Deactivate(Guid accountId)
    {
        _accountActivationService.DeactivateAccount(accountId);
        
        return NoContent();
    }
}