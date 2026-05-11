using Microsoft.AspNetCore.Mvc;
using RentalPipeline.Application.DTOs.Requests;
using RentalPipeline.Application.DTOs.Responses;
using RentalPipeline.Application.Interfaces.Repositories;
using RentalPipeline.Application.UseCases.Clients;

namespace RentalPipeline.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(
    CreateClientUseCase createClientUseCase,
    DeleteClientUseCase deleteClientUseCase,
    IClientRepository clientRepository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClientRequest request)
    {
        var response = await createClientUseCase.ExecuteAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var client = await clientRepository.GetByIdAsync(id);
        if (client is null) return NotFound();
        return Ok(ClientResponse.FromEntity(client));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clients = await clientRepository.GetAllAsync();
        return Ok(clients.Select(ClientResponse.FromEntity));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await deleteClientUseCase.ExecuteAsync(id);
        return NoContent();
    }
}