using Microsoft.AspNetCore.Mvc;
using RentalPipeline.Application.DTOs.Requests;
using RentalPipeline.Application.Interfaces.Repositories;
using RentalPipeline.Application.UseCases.Properties;
using RentalPipeline.Application.DTOs.Responses;

namespace RentalPipeline.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController(
    CreatePropertyUseCase createPropertyUseCase,
    IPropertyRepository propertyRepository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropertyRequest request)
    {
        var response = await createPropertyUseCase.ExecuteAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var property = await propertyRepository.GetByIdAsync(id);
        if (property is null) return NotFound();
        return Ok(PropertyResponse.FromEntity(property));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var properties = await propertyRepository.GetAllAsync();
        return Ok(properties.Select(PropertyResponse.FromEntity));
    }

    [HttpGet("owner/{ownerId:guid}")]
    public async Task<IActionResult> GetByOwner(Guid ownerId)
    {
        var properties = await propertyRepository.GetByOwnerIdAsync(ownerId);
        return Ok(properties.Select(PropertyResponse.FromEntity));
    }
}