using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalPipeline.Application.DTOs.Responses;
using RentalPipeline.Application.Interfaces.Repositories;
using RentalPipeline.Application.UseCases.Clients;
using RentalPipeline.Domain.Enums;

namespace RentalPipeline.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(
    DeleteClientUseCase deleteClientUseCase,
    IClientRepository clientRepository) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = ClientRole.AdminMaster)]
    public async Task<IActionResult> GetAll()
    {
        var clients = await clientRepository.GetAllAsync();
        return Ok(clients.Select(ClientResponse.FromEntity));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = ClientRole.AdminMaster)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var client = await clientRepository.GetByIdAsync(id);
        if (client is null) return NotFound();
        return Ok(ClientResponse.FromEntity(client));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = ClientRole.AdminMaster)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await deleteClientUseCase.ExecuteAsync(id);
        return NoContent();
    }
}