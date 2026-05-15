```md id="0qw7r1"
# Stack Técnica — [NOME DO PROJETO]

## Arquitetura

- Backend e frontend separados
- API REST
- n-tier MVC
- Estrutura modular
- Feature-based structure

---

# Estrutura de pastas
/app-keepcalm
    /src
        /backend
        /frontend

# Frontend

- Framework:
  - Svelte 5.55.5
  - SvelteKit
  - Node 24.X se necessário

- Linguagem:
  - TypeScript

- UI/CSS:
  - Bootstrap 5.3.8

- Estado:
  - Stores nativas do Svelte

- Forms:
  - Svelte FormKit

- Validação:
  - Zod

- Data fetching:
  - Fetch API

- Requisitos:
  - Responsivo
  - Mobile-first
  - Dark mode
  - Acessibilidade
  - SEO

---

# Backend

- Runtime:
  - .NET 10

- Framework:
  - ASP.NET Core 10

- Linguagem:
  - C#

- API:
  - REST

- Arquitetura:
  - N-tier MVC

- Auth:
  - JWT Bearer

- Validação:
  - Use "Data Annotations" para simplicidade

- Documentação:
  - Swagger / OpenAPI 3.2.0

---

# Banco de Dados

- Banco principal:
  - MongoDB

- ORM:
  - MongoDB Entity Framework Core Provider (https://www.mongodb.com/pt-br/docs/entity-framework/current/)
  - Pacote nugget (https://www.nuget.org/packages/MongoDB.EntityFrameworkCore)

- Convenções:
  - UUID como identificador
  - Collections por agregado
  - Soft delete
  - createdAt / updatedAt

---

# Infraestrutura

- Frontend:
  - Hospedagem em servidor local via container Docker

- Backend:
  - Hospedagem em servidor local via container Docker

- Banco:
  - Hospedagem em servidor local de MongoDB

- CI/CD:
  - GitHub Actions

---

# Segurança

- HTTPS obrigatório
- JWT obrigatório
- CORS configurado
- Sanitização de inputs
- Proteção contra:
  - XSS
  - CSRF
  - Injection attacks

- Secrets:
  - Variáveis de ambiente
  - .NET User Secrets em desenvolvimento

---

# Observabilidade

- Logs:
  - Microsoft.Extensions.Logging

- Monitoramento:
  - OpenTelemetry
  - Grafana

- Error tracking:
  - Logging

---

# Testes

- Backend:
  - xUnit
  - FluentAssertions

- Frontend:
  - Vitest

- E2E:
  - Playwright

---

# Convenções

- Nullable enabled
- TypeScript strict
- ESLint + Prettier
- Conventional Commits
- Código tipado
- Sem lógica duplicada
- Controllers leves
- Regras de negócio em Services

---

# Regras para IA

O agente deve:
- Seguir esta stack obrigatoriamente
- Utilizar .NET 10 e ASP.NET Core 10
- Utilizar Svelte 5.55.5 no frontend
- Utilizar MongoDB como banco principal
- Criar código tipado
- Tratar erros globalmente
- Criar testes
- Evitar dependências desnecessárias
- Não alterar arquitetura sem autorização
```
