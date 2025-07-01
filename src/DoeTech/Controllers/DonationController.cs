using DoeTech.DTOs;
using DoeTech.Models;
using DoeTech.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoeTech.Controllers;

[Route("rest/v{version:apiVersion}/donation")]
[ApiVersion("1")] // Define a versão da API
[Authorize]
public class DonationController(DonationService donationService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public IActionResult Add([FromBody] DonationDto request)
    {
        if (request == null) return BadRequest("The request body could not be null");

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var donation = donationService.Add(request);
        return Ok(donation);
    }

    [Authorize]
    [HttpGet]
    public IActionResult GetAll()
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var donations = donationService.GetAllDonations();

        return Ok(donations);
    }

    [Authorize]
    [HttpGet("{donationId}")]
    public IActionResult GetDonationById(Guid donationId)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var donation = donationService.GetDonationById(donationId);

        return Ok(donation);
    }

    [Authorize]
    [HttpPut("{donationId}")]
    public IActionResult Update(Guid donationId, [FromBody] DonationUpdateDto donationDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedDonation = donationService.UpdateDonation(donationId, donationDto);

        return Ok(updatedDonation);
    }

    [Authorize]
    [HttpPut("{donationId}/status")]
    public IActionResult UpdateStatus(Guid donationId, [FromBody] DonationStatusEnum status)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        donationService.UpdateStatus(donationId, status);

        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{donationId}")]
    public IActionResult Delete(Guid donationId)
    {
        donationService.RemoveDonationById(donationId);

        return Ok();
    }

    [Authorize]
    [HttpGet("query")]
    public IActionResult Query([FromQuery] DonationQueryDto filter)
    {
        var results = donationService.Query(filter);

        return Ok(results);
    }
}