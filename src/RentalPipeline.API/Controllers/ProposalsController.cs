using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalPipeline.Application.DTOs.Requests;
using RentalPipeline.Application.DTOs.Responses;
using RentalPipeline.Application.Interfaces.Repositories;
using RentalPipeline.Application.UseCases.Proposals;
using RentalPipeline.Domain.Enums;

namespace RentalPipeline.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProposalsController(
    CreateProposalUseCase createProposalUseCase,
    TransitionProposalUseCase transitionProposalUseCase,
    GetProposalHistoryUseCase getProposalHistoryUseCase,
    IProposalRepository proposalRepository) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateProposalRequest request)
    {
        var response = await createProposalUseCase.ExecuteAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var proposal = await proposalRepository.GetByIdAsync(id);
        if (proposal is null) return NotFound();
        return Ok(ProposalResponse.FromEntity(proposal));
    }

    [HttpGet]
    [Authorize(Roles = ClientRole.AdminMaster)]
    public async Task<IActionResult> GetAll()
    {
        var proposals = await proposalRepository.GetAllAsync();
        return Ok(proposals.Select(ProposalResponse.FromEntity));
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize]
    public async Task<IActionResult> Transition(Guid id, [FromBody] TransitionProposalRequest request)
    {
        var response = await transitionProposalUseCase.ExecuteAsync(id, request);
        return Ok(response);
    }

    [HttpGet("{id:guid}/history")]
    [Authorize]
    public async Task<IActionResult> GetHistory(Guid id)
    {
        var history = await getProposalHistoryUseCase.ExecuteAsync(id);
        return Ok(history);
    }
}