# 🏠 Rental Pipeline API

API REST para gerenciamento da esteira de contratos de aluguel da Auxiliadora Predial.

---

## 🌐 Ambientes em Produção

| Branch | Documentação | Link |
|--------|-------------|------|
| `main` — sem autenticação | Swagger | 🟢 [Acessar Swagger](https://teste-auxiliadora-production.up.railway.app/swagger/index.html) |
| `feature/authentication` — com JWT | Scalar | 🟢 [Acessar Scalar](https://endearing-upliftment-production-b672.up.railway.app/scalar/v1) |

> ⚠️ Ambos os ambientes estão com documentação interativa habilitada exclusivamente para fins de demonstração e avaliação técnica.

### Autenticação (branch feature/authentication)

**Login com o admin padrão:**
```json
POST /api/auth/login
{
  "email": "admin@admin.com",
  "password": "admin"
}
```

Copie o `token` retornado e use no Scalar clicando no cadeado 🔒, ou no header:
```
Authorization: Bearer {seu_token}
```

---

## Sumário

- [Sobre o Projeto](#sobre-o-projeto)
- [Tecnologias](#tecnologias)
- [Arquitetura](#arquitetura)
- [Como Rodar](#como-rodar)
- [Endpoints](#endpoints)
- [Autenticação e Permissões](#autenticação-e-permissões)
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
- **Swagger** — documentação interativa (branch main)
- **Scalar** — documentação interativa (branch feature/authentication)
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
│   └── Events/                    # ContractActivatedEvent, IEventPublisher
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

### 2. Escolha a branch

```bash
# Branch sem autenticação
git checkout main

# Branch com autenticação JWT
git checkout feature/authentication
```

### 3. Suba os containers

```bash
docker compose up --build
```

Isso irá:
- Subir o PostgreSQL
- Compilar e subir a API
- Rodar as migrations e criar as tabelas automaticamente
- Na branch `feature/authentication`: criar o usuário AdminMaster automaticamente

### 4. Acesse a documentação

```
# Branch main
http://localhost:5000/swagger

# Branch feature/authentication
http://localhost:5000/scalar/v1
```

### Rodar os testes

```bash
dotnet test
```

---

## Endpoints

### Auth *(branch feature/authentication)*

| Método | Rota | Permissão | Descrição |
|--------|------|-----------|-----------|
| POST | `/api/auth/register` | Público | Cadastra um usuário |
| POST | `/api/auth/login` | Público | Autentica e retorna token JWT |
| GET | `/api/auth/me` | Autenticado | Retorna dados do usuário logado |

### Clients

| Método | Rota | Permissão | Descrição |
|--------|------|-----------|-----------|
| GET | `/api/clients` | AdminMaster | Lista todos os clientes |
| GET | `/api/clients/{id}` | AdminMaster | Busca cliente por ID |
| DELETE | `/api/clients/{id}` | AdminMaster | Remove um cliente |

### Properties

| Método | Rota | Permissão | Descrição |
|--------|------|-----------|-----------|
| POST | `/api/properties` | Autenticado | Cadastra um imóvel |
| GET | `/api/properties` | Público | Lista todos os imóveis |
| GET | `/api/properties/{id}` | Público | Busca imóvel por ID |
| GET | `/api/properties/owner/{ownerId}` | Público | Lista imóveis por proprietário |

### Proposals

| Método | Rota | Permissão | Descrição |
|--------|------|-----------|-----------|
| POST | `/api/proposals` | Autenticado | Cria uma proposta |
| GET | `/api/proposals` | AdminMaster | Lista todas as propostas |
| GET | `/api/proposals/{id}` | Autenticado | Busca proposta por ID |
| PATCH | `/api/proposals/{id}/status` | Autenticado | Avança o status da proposta |
| GET | `/api/proposals/{id}/history` | Autenticado | Retorna o histórico de transições |

---

## Autenticação e Permissões

> Disponível na branch `feature/authentication`

### Roles

| Role | Como obter | Permissões |
|------|-----------|------------|
| `User` | Qualquer registro via `/api/auth/register` | Criar imóveis, criar propostas, avançar status, ver histórico |
| `AdminMaster` | Criado automaticamente na inicialização | Tudo acima + listar todos clientes/propostas, deletar clientes |

### Regras de negócio

- Todo usuário registrado nasce com role `User`
- O `AdminMaster` é criado automaticamente na primeira inicialização
- Um mesmo usuário pode ser dono de imóveis e inquilino de outros simultaneamente
- O proprietário não pode criar proposta para o próprio imóvel
- `document` (CPF) não é exposto em rotas públicas — apenas em rotas autenticadas

---

## Exemplos de uso

**Registrar usuário:**
```json
POST /api/auth/register
{
  "name": "João Silva",
  "email": "joao@email.com",
  "document": "123.456.789-00",
  "password": "senha123"
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
             ↘                   ↘                ↘
         REPROVADA           CANCELADA         CANCELADA
```

---

## Decisões de Design

### Por que Clean Architecture?

A separação em camadas garante que as regras de negócio (Domain) não dependam de detalhes de infraestrutura. É possível trocar o banco de dados ou o framework HTTP sem tocar nas regras de negócio. Também facilita os testes unitários — o Domain é testado sem nenhuma dependência externa.

### Por que PostgreSQL?

PostgreSQL oferece suporte nativo ao `SELECT FOR UPDATE`, essencial para a implementação do lock pessimista. Também é o banco relacional mais adotado no mercado atualmente.

### Por que Lock Pessimista?

O sistema deve garantir que dois clientes não consigam criar propostas para o mesmo imóvel simultaneamente. O lock pessimista (`SELECT FOR UPDATE`) bloqueia a row no banco durante a transação, impedindo que outro request leia o imóvel como "Disponível" antes do primeiro commit.

### Por que a Máquina de Estados está no Domain?

A lógica de transição de estados é uma regra de negócio central. Ao manter o `AllowedTransitions` e o método `TransitionTo` dentro da entidade `Proposal`, garantimos que é impossível colocar uma proposta em estado inválido, independente de qual camada chame o método.

### Arquitetura Orientada a Eventos

Quando uma proposta atinge o status `ATIVO`, o sistema publica um `ContractActivatedEvent` via `IEventPublisher`. A implementação atual loga o evento estruturadamente no console. Em produção, bastaria trocar o `ConsoleEventPublisher` por uma implementação que publique em RabbitMQ, AWS SNS ou Azure Service Bus — sem alterar nenhuma regra de negócio.

### Por que FluentValidation?

FluentValidation é mais legível, testável e extensível que Data Annotations. As regras ficam em classes separadas, respeitando o princípio de responsabilidade única.

### Por que JWT?

JWT permite autenticação stateless — o servidor não precisa armazenar sessões. O token carrega as claims do usuário (id, email, role) e é validado pela chave secreta a cada request. BCrypt garante que as senhas nunca sejam armazenadas em texto plano.

---

## Fluxo de Status do Imóvel

| Evento | Status anterior | Status novo |
|--------|----------------|-------------|
| Proposta criada | Disponível | Em Negociação |
| Proposta ATIVA | Em Negociação | Alugado (permanente) |
| Proposta REPROVADA | Em Negociação | Disponível |
| Proposta CANCELADA | Em Negociação | Disponível |