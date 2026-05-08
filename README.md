# Rental Pipeline API

API REST para gerenciamento da esteira de contratos de aluguel da Auxiliadora Predial.

---

# 🏠 Rental Pipeline API

> **API em produção e disponível para testes**
>
> 🟢 **[Acessar Swagger — Ambiente de Demonstração](https://teste-auxiliadora-production.up.railway.app/swagger/index.html)**
>
> ⚠️ O Swagger está habilitado em produção exclusivamente para fins de demonstração e avaliação técnica.

---

## Sumário

- [Sobre o Projeto](#sobre-o-projeto)
- [Tecnologias](#tecnologias)
- [Arquitetura](#arquitetura)
- [Como Rodar](#como-rodar)
- [Endpoints](#endpoints)
- [Decisões de Design](#decisões-de-design)
- [Testes](#testes)

---

## Sobre o Projeto

O sistema gerencia a jornada de uma proposta de locação desde a intenção do cliente até a assinatura final do contrato, garantindo que:

- Um imóvel não pode ser alugado por duas pessoas ao mesmo tempo
- As etapas do contrato respeitam uma ordem lógica e imutável
- O proprietário não pode alugar o próprio imóvel
- Toda transição de status é registrada com data e hora (audit trail)

---

## Tecnologias

- **.NET 10** — plataforma principal
- **PostgreSQL 16** — banco de dados relacional
- **Entity Framework Core** — ORM e migrations
- **FluentValidation** — validação de payloads
- **xUnit + FluentAssertions** — testes unitários
- **Swagger** — documentação interativa da API
- **Docker + Docker Compose** — containerização

---

## Arquitetura

O projeto segue **Clean Architecture** com 4 camadas:

```
src/
├── RentalPipeline.Domain          # Entidades, enums, regras de negócio
│   ├── Entities/                  # Property, Client, Proposal, ProposalHistory
│   ├── Enums/                     # ProposalStatus, PropertyStatus
│   └── Exceptions/                # InvalidTransitionException, PropertyNotAvailableException
│
├── RentalPipeline.Application     # Casos de uso, DTOs, interfaces
│   ├── UseCases/                  # CreateProposal, TransitionProposal, etc.
│   ├── DTOs/                      # Requests e Responses
│   ├── Interfaces/                # IPropertyRepository, IUnitOfWork, IEventPublisher
│   ├── Validators/                # FluentValidation validators
│   └── Events/                   # ContractActivatedEvent, IEventPublisher
│
├── RentalPipeline.Infrastructure  # EF Core, repositórios, eventos
│   ├── Data/                      # AppDbContext, Mappings
│   ├── Repositories/              # Implementações dos repositórios
│   └── Events/                    # ConsoleEventPublisher
│
└── RentalPipeline.API             # Controllers, middlewares, Program.cs
    ├── Controllers/               # PropertiesController, ClientsController, ProposalsController
    └── Middlewares/               # ExceptionMiddleware, ValidationFilter

tests/
└── RentalPipeline.Tests           # Testes unitários
    └── Domain/                    # ProposalStateMachineTests
```

**Princípio de dependência:** cada camada só conhece a camada imediatamente abaixo. O Domain não depende de nada externo.

---

## Como Rodar

### Pré-requisitos

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/)

### 1. Clone o repositório

```bash
git clone https://github.com/eminesf/teste-auxiliadora.git
cd teste-auxiliadora
```

### 2. Suba os containers

```bash
docker compose up --build
```

Isso irá:
- Subir o PostgreSQL
- Compilar e subir a API
- Rodar as migrations e criar as tabelas automaticamente

### 3. Acesse a documentação

```
http://localhost:5000/swagger
```

### Rodar os testes

```bash
dotnet test
```

---

## Endpoints

### Clients

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/clients` | Cadastra um cliente |
| GET | `/api/clients` | Lista todos os clientes |
| GET | `/api/clients/{id}` | Busca cliente por ID |

### Properties

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/properties` | Cadastra um imóvel |
| GET | `/api/properties` | Lista todos os imóveis |
| GET | `/api/properties/{id}` | Busca imóvel por ID |
| GET | `/api/properties/owner/{ownerId}` | Lista imóveis por proprietário |

### Proposals

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/proposals` | Cria uma proposta |
| GET | `/api/proposals` | Lista todas as propostas |
| GET | `/api/proposals/{id}` | Busca proposta por ID |
| PATCH | `/api/proposals/{id}/status` | Avança o status da proposta |
| GET | `/api/proposals/{id}/history` | Retorna o histórico de transições |

### Exemplos de uso

**Criar cliente:**
```json
POST /api/clients
{
  "name": "João Silva",
  "email": "joao@email.com",
  "document": "123.456.789-00"
}
```

**Criar imóvel:**
```json
POST /api/properties
{
  "ownerId": "uuid-do-owner",
  "street": "Rua das Flores",
  "number": "123",
  "complement": "Apto 42",
  "district": "Moinhos de Vento",
  "city": "Porto Alegre",
  "state": "RS",
  "rentPrice": 2500.00
}
```

**Criar proposta:**
```json
POST /api/proposals
{
  "propertyId": "uuid-do-imovel",
  "clientId": "uuid-do-cliente"
}
```

**Avançar status:**
```json
PATCH /api/proposals/{id}/status
{
  "newStatus": "AnaliseCredito"
}
```

**Fluxo completo de estados:**
```
NOVA → ANALISE_CREDITO → CONTRATO_EMITIDO → ASSINADO → ATIVO
                ↘                    ↘              ↘
            REPROVADA            CANCELADA       CANCELADA
```

---

## Decisões de Design

### Por que Clean Architecture?

A separação em camadas garante que as regras de negócio (Domain) não dependam de detalhes de infraestrutura. É possível trocar o banco de dados ou o framework HTTP sem tocar nas regras de negócio. Também facilita os testes unitários — o Domain é testado sem nenhuma dependência externa.

### Por que PostgreSQL?

PostgreSQL oferece suporte nativo ao `SELECT FOR UPDATE`, essencial para a implementação do lock pessimista. Também é o banco relacional mais adotado no mercado atualmente.

### Por que Lock Pessimista?

O sistema deve garantir que dois clientes não consigam criar propostas para o mesmo imóvel simultaneamente. O lock pessimista (`SELECT FOR UPDATE`) bloqueia a row no banco durante a transação, impedindo que outro request leia o imóvel como "Disponível" antes do primeiro commit. Isso é mais adequado para este cenário de alta importância de integridade do que o lock otimista.

### Por que a Máquina de Estados está no Domain?

A lógica de transição de estados é uma regra de negócio central — não é responsabilidade do banco nem do controller decidir se uma transição é válida. Ao manter o `AllowedTransitions` e o método `TransitionTo` dentro da entidade `Proposal`, garantimos que é **impossível** colocar uma proposta em estado inválido, independente de qual camada chame o método.

### Arquitetura Orientada a Eventos

Quando uma proposta atinge o status `ATIVO`, o sistema publica um `ContractActivatedEvent` via `IEventPublisher`. A implementação atual loga o evento estruturadamente no console. Em produção, bastaria trocar o `ConsoleEventPublisher` por uma implementação que publique em RabbitMQ, AWS SNS ou Azure Service Bus — sem alterar nenhuma regra de negócio.

### Por que FluentValidation?

FluentValidation é mais legível, testável e extensível que Data Annotations. As regras ficam em classes separadas, respeitando o princípio de responsabilidade única. Também permite mensagens de erro em português sem anotações no modelo.

---

## Fluxo de Status do Imóvel

| Evento | Status anterior | Status novo |
|--------|----------------|-------------|
| Proposta criada | Disponível | Em Negociação |
| Proposta ATIVA | Em Negociação | Alugado (permanente) |
| Proposta REPROVADA | Em Negociação | Disponível |
| Proposta CANCELADA | Em Negociação | Disponível |