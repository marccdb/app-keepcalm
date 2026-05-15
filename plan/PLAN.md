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
