# Fase 1 — Plano de Execucao (MVP)

> CRUD de tarefas com prioridade, micro-passos, progresso visual basico, timer de foco, MongoDB, design calmo.

---

## Tarefas

### ✅ 1. Backend Scaffolding

**Status:** COMPLETO

**Arquivos:**
- `src/backend/KeepCalm.sln`
- `src/backend/KeepCalm/KeepCalm.csproj`
- `src/backend/KeepCalm/Program.cs`
- `src/backend/KeepCalm/Dockerfile`
- `.dockerignore`
- `.env.example`

---

### ✅ 2. MongoDB Context e Modelos

**Status:** COMPLETO

**Arquivos:**
- `src/backend/KeepCalm/Models/Task.cs`
- `src/backend/KeepCalm/Models/MicroStep.cs`
- `src/backend/KeepCalm/Models/FocusSession.cs`
- `src/backend/KeepCalm/Data/MongoDbContext.cs`

**Observacoes:** `ToCollection` extension method requer `using MongoDB.EntityFrameworkCore.Extensions;` (nao vem com `using MongoDB.EntityFrameworkCore;`).

---

### ✅ 3. Task CRUD API

**Descricao:** Endpoint completo de tarefas
- `GET /api/tasks` — listar com filtro por prioridade
- `POST /api/tasks` — criar tarefa
- `PUT /api/tasks/{id}` — atualizar tarefa
- `PATCH /api/tasks/{id}/reorder` — reordenar
- `DELETE /api/tasks/{id}` — soft delete
- Service layer com regras de negocio
- DTOs com validacao Data Annotations

**Arquivos:**
- `src/backend/KeepCalm/DTOs/TaskDto.cs`
- `src/backend/KeepCalm/DTOs/MicroStepDto.cs`
- `src/backend/KeepCalm/Services/ITaskService.cs`
- `src/backend/KeepCalm/Services/TaskService.cs`
- `src/backend/KeepCalm/Controllers/TasksController.cs`

---

### 4. Micro-steps API

**Descricao:** Endpoint para quebrar tarefas em micro-passos
- `POST /api/tasks/{taskId}/micro-steps` — adicionar micro-passo
- `PATCH /api/micro-steps/{id}` — atualizar progresso
- `GET /api/tasks/{taskId}/micro-steps` — listar micro-passos de uma tarefa
- Service layer

**Arquivos:**
- `src/backend/KeepCalm/Services/IMicroStepService.cs`
- `src/backend/KeepCalm/Services/MicroStepService.cs`
- `src/backend/KeepCalm/Controllers/MicroStepsController.cs`

---

### 5. Focus Timer API

**Descricao:** Endpoint para timer de foco (Pomodoro adaptativo)
- `POST /api/focus-sessions/start` — iniciar sessao
- `PATCH /api/focus-sessions/{id}/pause` — pausar
- `PATCH /api/focus-sessions/{id}/resume` — retomar
- `POST /api/focus-sessions/{id}/complete` — completar
- `GET /api/focus-sessions` — historico

**Arquivos:**
- `src/backend/KeepCalm/Services/IFocusSessionService.cs`
- `src/backend/KeepCalm/Services/FocusSessionService.cs`
- `src/backend/KeepCalm/Controllers/FocusSessionsController.cs`
- `src/backend/KeepCalm/Jobs/FocusReminderJob.cs`

---

### ✅ 6. Frontend Scaffolding

**Status:** COMPLETO

- [x] Inicializar projeto SvelteKit com TypeScript (`sv create`)
- [x] Instalar Bootstrap 5.3.8 + @popperjs/core
- [x] Instalar ESLint + typescript-eslint + prettier
- [x] Configurar vite.config.ts com API proxy para `localhost:5000`
- [x] Configurar `eslint.config.js` com suporte a Svelte
- [x] Configurar `.prettierrc`
- [x] Configurar scripts `lint`, `lint:fix`, `format` no package.json
- [x] Atualizar `app.html` com Google Fonts (Inter) + lang pt-BR
- [x] Atualizar `+layout.svelte` com import global CSS

**Arquivos criados:**
- `src/frontend/package.json`
- `src/frontend/svelte.config.js`
- `src/frontend/vite.config.ts`
- `src/frontend/tsconfig.json`
- `src/frontend/eslint.config.js`
- `src/frontend/.prettierrc`
- `src/frontend/src/app.html`
- `src/frontend/src/routes/+layout.svelte`
- `src/frontend/src/routes/+page.svelte`

---

### ✅ 7. Calm UI Design System

**Status:** COMPLETO

- [x] Variaveis CSS customizadas (cores suaves, sombras leves, bordas, radius)
- [x] Tipografia calma (Inter via Google Fonts)
- [x] Animacoes suaves (fade-in, pulse, breathing)
- [x] Componentes CSS reutilizaveis:
  - `.calm-card` — card com sombra e hover
  - `.calm-container` — container max-width
  - `.calm-header` / `.calm-nav` — navegacao
  - `.calm-btn-*` — botoes com variantes (primary, secondary, success, danger)
  - `.calm-badge-*` — badges de prioridade/estado
  - `.calm-progress-*` — barra de progresso
  - `.calm-list-*` — lista estilizada
  - `.calm-empty` — estado vazio
  - `.calm-divider`, `.calm-tooltip`
- [x] Import global via `src/css/app.css` (Bootstrap + calm.css)
- [x] Responsividade basica
- [x] Componentes Svelte reutilizaveis:
  - `PriorityBadge.svelte` — badge de prioridade
  - `ProgressBar.svelte` — barra de progresso com label
  - `TaskCard.svelte` — card de tarefa completo
  - `Modal.svelte` — modal calmo para criar/editar

**Arquivos criados:**
- `src/frontend/src/css/calm.css`
- `src/frontend/src/css/app.css`
- `src/frontend/src/lib/components/PriorityBadge.svelte`
- `src/frontend/src/lib/components/ProgressBar.svelte`
- `src/frontend/src/lib/components/TaskCard.svelte`
- `src/frontend/src/lib/components/Modal.svelte`
- `src/frontend/src/lib/components/index.ts`
- `src/frontend/src/lib/components/TaskCard.svelte`
- `src/frontend/src/lib/components/Button.svelte`
- `src/frontend/src/lib/components/Modal.svelte`

---

### ✅ 8. Frontend Pages

**Status:** COMPLETO (svelte-check: 0 errors)

- [x] Dashboard (`+page.svelte`) — lista de tarefas pendentes/concluidas, modal de criacao
- [x] TaskDetail (`task/[id]/+page.svelte`) — detalhes da tarefa, micro-passos
- [x] Timer (`timer/+page.svelte`) — timer de foco com presets (25min/5min/15min)
- [x] Settings (`settings/+page.svelte`) — configuracoes do timer e preferencias
- [x] Layout responsivo mobile-first
- [x] Navegacao entre paginas (Dashboard, Timer, Settings)
- [x] Estados: loading, empty, error
- [x] Svelte 5 compatibility (onclick, onsubmit, explicit props)

**Arquivos criados:**
- `src/frontend/src/routes/+layout.svelte`
- `src/frontend/src/routes/+page.svelte` (Dashboard)
- `src/frontend/src/routes/task/[id]/+page.svelte` (TaskDetail)
- `src/frontend/src/routes/timer/+page.svelte`
- `src/frontend/src/routes/settings/+page.svelte`

---

### ✅ 9. Docker Compose

**Status:** COMPLETO

- [x] Service `mongo` — MongoDB 8.0 com volume
- [x] Service `backend` — .NET 10 ASP.NET com exposed port 5000
- [x] Service `frontend` — SvelteKit dev server com exposed port 5173
- [x] Network compartilhada `keepcalm`
- [x] Volumes: `mongo-data`, `backend-logs`
- [x] Dependencias (`depends_on`)
- [x] Backend Dockerfile multi-stage
- [x] Frontend Dockerfile multi-stage

**Arquivos criados:**
- `docker-compose.yml`
- `src/backend/KeepCalm/Dockerfile`
- `src/frontend/Dockerfile`

---

### 10. Tests

**Descricao:** Testes basicos
- Backend: xUnit tests para TaskService e MicroStepService
- Frontend: Vitest para stores e componentes
- FluentAssertions para assertions

**Arquivos:**
- `src/backend/KeepCalm.Tests/TaskServiceTests.cs`
- `src/backend/KeepCalm.Tests/MicroStepServiceTests.cs`
- `src/frontend/tests/stores/tasks.test.ts`
- `src/frontend/tests/components/ProgressBar.test.ts`

---

## Ordem de execucao

1 → 2 → 3 → 4 → 5 → 6 → 7 → 8 → 9 → 10

Cada tarefa depende da anterior (backend precisa de modelo, frontend depende de API).

## Entregavel da Fase 1

- CRUD de tarefas com 4 niveis de prioridade
- Micro-passos manuais (sem IA ainda)
- Barra de progresso visual
- Timer de foco funcional
- MongoDB rodando via Docker
- Interface responsiva, tons claros, design calmo
