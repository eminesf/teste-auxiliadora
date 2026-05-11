using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalPipeline.Application.DTOs.Requests;
using RentalPipeline.Application.DTOs.Responses;
using RentalPipeline.Application.Interfaces.Repositories;
using RentalPipeline.Application.UseCases.Properties;

namespace RentalPipeline.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController(
    CreatePropertyUseCase createPropertyUseCase,
    IPropertyRepository propertyRepository) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreatePropertyRequest request)
    {
        var response = await createPropertyUseCase.ExecuteAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var properties = await propertyRepository.GetAllAsync();
        return Ok(properties.Select(PropertyResponse.FromEntity));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var property = await propertyRepository.GetByIdAsync(id);
        if (property is null) return NotFound();
        return Ok(PropertyResponse.FromEntity(property));
    }

    [HttpGet("owner/{ownerId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByOwner(Guid ownerId)
    {
        var properties = await propertyRepository.GetByOwnerIdAsync(ownerId);
        return Ok(properties.Select(PropertyResponse.FromEntity));
    }
}