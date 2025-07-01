using DoeTech.DTOs;
using DoeTech.Models;
using DoeTech.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoeTech.Controllers;

[Route("rest/v{version:apiVersion}/equipment")]
[ApiVersion("1")] // Define a versão da API
[Authorize]
public class EquipmentController(EquipmentService equipmentService) : ControllerBase
{
    [HttpPost]
    public IActionResult Add([FromBody] Equipment request)
    {
        if (request == null)
        {
            BadRequest("The request body could not be null.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }


        var equipment = equipmentService.AddEquipment(request);
        return Ok(equipment);
    }

    [HttpGet("{equipmentId}")]
    public IActionResult GetEquipmentById(Guid equipmentId)
    {
        var equipment = equipmentService.GetEquipmentById(equipmentId);

        return Ok(equipment);
    }

    [HttpGet]
    public IActionResult GetEquipments()
    {
        var equipments = equipmentService.GetEquipments();

        return Ok(equipments);
    }
    
    [HttpGet("query")]
    public IActionResult Query([FromQuery] EquipmentQueryDto query)
    {
        var equipments = equipmentService.Query(query);

        return Ok(equipments);
    }

    [Authorize]
    [HttpPut("{equipmentId}/status")]
    public IActionResult UpdateEquipmentAvailability(Guid equipmentId, [FromBody] EquipmentStatusDTO status)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Console.WriteLine(status.Status);
        var equipment = equipmentService.UpdateEquipmentStatus(equipmentId, status.Status);

        return Ok(equipment);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete]
    public IActionResult Remove(Guid equipmentId)
    {

        equipmentService.RemoveEquipment(equipmentId);

        return NoContent();
    }
}