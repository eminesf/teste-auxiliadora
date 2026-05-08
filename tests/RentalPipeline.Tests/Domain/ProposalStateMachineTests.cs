using FluentAssertions;
using RentalPipeline.Domain.Entities;
using RentalPipeline.Domain.Enums;
using RentalPipeline.Domain.Exceptions;

namespace RentalPipeline.Tests.Domain;

public class ProposalStateMachineTests
{
  // ── Fluxo feliz ────────────────────────────────────────────────────────

  [Fact]
  public void TransitionTo_ShouldSucceed_WhenFollowingHappyPath()
  {
    var (proposal, _) = ProposalBuilder.Build();

    proposal.TransitionTo(ProposalStatus.AnaliseCredito);
    proposal.TransitionTo(ProposalStatus.ContratoEmitido);
    proposal.TransitionTo(ProposalStatus.Assinado);
    proposal.TransitionTo(ProposalStatus.Ativo);

    proposal.Status.Should().Be(ProposalStatus.Ativo);
  }

  [Fact]
  public void NewProposal_ShouldStartWithStatusNova()
  {
    var (proposal, _) = ProposalBuilder.Build();

    proposal.Status.Should().Be(ProposalStatus.Nova);
  }

  // ── Transições inválidas ────────────────────────────────────────────────

  [Theory]
  [InlineData(ProposalStatus.Nova, ProposalStatus.ContratoEmitido)]
  [InlineData(ProposalStatus.Nova, ProposalStatus.Assinado)]
  [InlineData(ProposalStatus.Nova, ProposalStatus.Ativo)]
  [InlineData(ProposalStatus.AnaliseCredito, ProposalStatus.Assinado)]
  [InlineData(ProposalStatus.AnaliseCredito, ProposalStatus.Ativo)]
  [InlineData(ProposalStatus.ContratoEmitido, ProposalStatus.Ativo)]
  public void TransitionTo_ShouldThrow_WhenSkippingSteps(
      ProposalStatus from, ProposalStatus to)
  {
    var (proposal, _) = ProposalBuilder.Build();

    // Avança até o estado inicial do teste
    AdvanceTo(proposal, from);

    var act = () => proposal.TransitionTo(to);

    act.Should().Throw<InvalidTransitionException>();
  }

  [Theory]
  [InlineData(ProposalStatus.Ativo)]
  [InlineData(ProposalStatus.Reprovada)]
  [InlineData(ProposalStatus.Cancelada)]
  public void TransitionTo_ShouldThrow_WhenLeavingFinalState(ProposalStatus finalState)
  {
    var (proposal, _) = ProposalBuilder.Build();

    AdvanceTo(proposal, finalState);

    var act = () => proposal.TransitionTo(ProposalStatus.Nova);

    act.Should().Throw<InvalidTransitionException>();
  }

  // ── Efeitos colaterais no imóvel ────────────────────────────────────────

  [Fact]
  public void TransitionTo_Ativo_ShouldSetPropertyStatusToRented()
  {
    var (proposal, property) = ProposalBuilder.Build();

    AdvanceTo(proposal, ProposalStatus.Ativo);

    property.Status.Should().Be(PropertyStatus.Rented);
  }

  [Fact]
  public void TransitionTo_Reprovada_ShouldSetPropertyStatusToAvailable()
  {
    var (proposal, property) = ProposalBuilder.Build();

    proposal.TransitionTo(ProposalStatus.Reprovada);

    property.Status.Should().Be(PropertyStatus.Available);
  }

  [Fact]
  public void TransitionTo_Cancelada_ShouldSetPropertyStatusToAvailable()
  {
    var (proposal, property) = ProposalBuilder.Build();

    proposal.TransitionTo(ProposalStatus.Cancelada);

    property.Status.Should().Be(PropertyStatus.Available);
  }

  [Fact]
  public void TransitionTo_Cancelada_FromAssinado_ShouldSetPropertyStatusToAvailable()
  {
    var (proposal, property) = ProposalBuilder.Build();

    AdvanceTo(proposal, ProposalStatus.Assinado);
    proposal.TransitionTo(ProposalStatus.Cancelada);

    property.Status.Should().Be(PropertyStatus.Available);
  }

  // ── Helper ─────────────────────────────────────────────────────────────

  private static void AdvanceTo(Proposal proposal, ProposalStatus target)
  {
    var path = new[]
    {
            ProposalStatus.Nova,
            ProposalStatus.AnaliseCredito,
            ProposalStatus.ContratoEmitido,
            ProposalStatus.Assinado,
            ProposalStatus.Ativo
        };

    // Estados de saída que não estão no caminho feliz
    if (target == ProposalStatus.Reprovada)
    {
      proposal.TransitionTo(ProposalStatus.Reprovada);
      return;
    }

    if (target == ProposalStatus.Cancelada)
    {
      proposal.TransitionTo(ProposalStatus.Cancelada);
      return;
    }

    foreach (var status in path.SkipWhile(s => s != proposal.Status).Skip(1))
    {
      proposal.TransitionTo(status);
      if (proposal.Status == target) break;
    }
  }
}