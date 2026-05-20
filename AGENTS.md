# App KeepCalm — Plano do Projeto

> Web app para gerenciamento de tarefas diárias de programacao, projetado para pessoas ansiosas.

---

## Visao geral

App web que reduz a sobrecarga mental ao gerenciar tarefas, com design calmo em tons claros, funcionalidades que quebram o peso emocional de "muita coisa para fazer" e dados persistentes em MongoDB.

---

## Principios de design

- **Sem culpa**: dias sem completar tarefas nao sao fracasso, sao dados validos
- **Micro-passos**: nada grande demais — quebrar ate ficar leve
- **Progresso visual**: cada avancozinho e visivel e celebrativo
- **Suave, nao invasivo**: lembretes que parecem sugestoes de um amigo
- **Minimalista por padrao**: so o necessario aparecendo por vez
- **Design calmo em tons claros**: interface leve, sem contraste agressivo

---

## Funcionalidades

### 1. Tarefas por Prioridade
- Classificacao: urgente, importante, normal, baixa
- Filtro automatico mostra apenas o necessario agora
- Reordenacao inteligente (sugere proximo passo menos esmagador)

### 2. Quebra em Micro-passos
- Ao criar tarefa, pergunta: "como quebrar isso?"
- Sugerido por IA via endpoint local (usuario roda IA localmente)
- Barras de progresso por passo individual

### 3. Visual Progressivo
- Barra de progresso principal do dia
- Check animado suave (sem confetti agressivo — algo calmo)
- Dashboard semanal: "voce fez X de Y. Descanse no que falta"

### 4. Lembretes Suaves
- Timer de foco (Pomodoro adaptativo — usuario define o ritmo)
- Pausas guiadas com respiracao
- Notificacoes: "se quiser, agora da pra fazer um passo" (nunca "voce DEVE")
- Opcao "nao agora" sem julgamento

### 5. Journaling/Reflexao noturna
- Prompt diario: "como foi seu dia?"
- Opcoes rapidas + texto livre
- Revisao semanal: "esta semana, X tarefas completadas. Principal desafio: ..."
- Opcao de "amanha quero focar em..."

### 6. Estado emocional
- Check-in rapido ao abrir: "como se sente?"
- Ajusta sugerir tarefas menores se ansiedade alta
- Registro historico para auto-observacao

---

## Faseamento

### Fase 1: Base (MVP)
- CRUD de tarefas com prioridade
- Quebra em micro-passos (manual + IA local)
- Progresso visual basico
- Timer de foco simples
- Persistencia em MongoDB
- Design calmo em tons claros

### Fase 2: Reflexao
- Journaling noturno
- Dashboard semanal
- Check-in emocional
- Revisao semanal automatica

### Fase 3: Polimento
- Lembretes suaves (notifications)
- IA para sugerir quebra de tarefas
- Sincronizacao cloud (se necessario)
- PWA / instalavel

---

## Perguntas respondidas

| Pergunta | Resposta |
|---|---|
| Persistencia | MongoDB (banco de dados) |
| IA para tarefas | Sim, via endpoint local |
| Design | Calmo, tons claros |
| Stack tecnica | Ver arquivo de instrucoes tecnicas separado |


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
