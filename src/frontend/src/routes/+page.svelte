<script lang="ts">
	import { onMount } from 'svelte';
	import { goto } from '$app/navigation';
	import TaskCard from '$lib/components/TaskCard.svelte';
	import { Modal } from '$lib/components/index.js';
	import PriorityBadge from '$lib/components/PriorityBadge.svelte';
	import ThemeToggle from '$lib/components/ThemeToggle.svelte';
	import { fetchTasks, addTask, toggleTask, tasks, loading, error } from '$lib/stores/tasks';

	let showCreateModal = $state(false);
	let newTask = $state<{ title: string; description: string; priority: 'urgent' | 'important' | 'normal' | 'low' }>({
		title: '',
		description: '',
		priority: 'normal'
	});

	let createError = $state<string | null>(null);

	const priorityOptions = [
		{ value: 'low', label: 'Baixa', color: 'info' },
		{ value: 'normal', label: 'Normal', color: 'primary' },
		{ value: 'important', label: 'Importante', color: 'warning' },
		{ value: 'urgent', label: 'Urgente', color: 'danger' }
	] as const;

	$effect(() => {
		fetchTasks();
	});

	$effect(() => {
		if ($error) {
			createError = $error;
		}
	});

	async function handleCreate() {
		if (!newTask.title.trim()) return;

		createError = null;
		await addTask({
			title: newTask.title,
			description: newTask.description,
			priority: newTask.priority
		});

		newTask = { title: '', description: '', priority: 'normal' };
		showCreateModal = false;
	}

	function handleToggle(id: string) {
		toggleTask(id);
	}
</script>

<div class="calm-header">
	<div class="calm-container">
		<div class="calm-nav" style="justify-content: space-between;">
			<div style="display: flex; align-items: center; gap: 0.75rem;">
				<span style="font-size: 1.4rem; font-weight: 700; color: var(--calm-primary);">
					✦
				</span>
				<span style="font-weight: 600; color: var(--calm-text);">KeepCalm</span>
			</div>
			<div style="display: flex; align-items: center; gap: 0.5rem;">
				<nav style="display: flex; gap: 0.25rem;">
					<a href="/" class="calm-nav-link active">Dashboard</a>
					<a href="/timer" class="calm-nav-link">Timer</a>
					<a href="/settings" class="calm-nav-link">Configuracoes</a>
				</nav>
				<ThemeToggle />
			</div>
		</div>
	</div>
</div>

<div class="calm-container calm-section">
	<!-- Header -->
	<div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 2rem; flex-wrap: wrap; gap: 1rem;">
		<div>
			<h1 style="font-size: 1.8rem; font-weight: 700; color: var(--calm-text); margin: 0 0 0.25rem;">
				Dashboard
			</h1>
			{#if !$loading}
				<p style="font-size: 0.95rem; color: var(--calm-text-secondary); margin: 0;">
					{$tasks.filter((t) => !t.completed).length} tarefas pendentes · {$tasks.filter((t) => t.completed).length} concluidas
				</p>
			{/if}
		</div>
		<button class="calm-btn calm-btn-primary calm-btn-lg" onclick={() => (showCreateModal = true)}>
			+ Nova Tarefa
		</button>
	</div>

	<!-- Error state -->
	{#if $error}
		<div class="alert alert-danger" role="alert">
			{$error}
		</div>
	{/if}

	<!-- Loading state -->
	{#if $loading}
		<div class="calm-empty">
			<div class="calm-empty-icon calm-pulse">⏳</div>
			<p class="calm-empty-text">Carregando tarefas...</p>
		</div>
	{:else if $tasks.length === 0}
		<!-- Empty state -->
		<div class="calm-empty calm-fade-in">
			<div class="calm-empty-icon">🌿</div>
			<p class="calm-empty-text">Nenhuma tarefa ainda</p>
			<p class="calm-empty-hint">Clique em "Nova Tarefa" para começar.</p>
		</div>
	{:else}
		<!-- Active tasks -->
		{#if $tasks.filter((t) => !t.completed).length > 0}
			<h2 style="font-size: 1.15rem; font-weight: 600; color: var(--calm-text); margin-bottom: 1rem;">
				Pendentes
			</h2>
			<div style="display: flex; flex-direction: column; gap: 0.75rem; margin-bottom: 2rem;">
				{#each $tasks.filter((t) => !t.completed) as task (task.id)}
					<TaskCard
						id={task.id}
						title={task.title}
						description={task.description}
						priority={task.priority}
						progress={task.progress}
					onClick={() => goto(`/task/${task.id}`)}
						onToggle={() => handleToggle(task.id)}
					/>
				{/each}
			</div>
		{/if}

		<!-- Completed tasks -->
		{#if $tasks.filter((t) => t.completed).length > 0}
			<h2 style="font-size: 1.15rem; font-weight: 600; color: var(--calm-text); margin-bottom: 1rem;">
				Concluidas
			</h2>
			<div style="display: flex; flex-direction: column; gap: 0.75rem;">
				{#each $tasks.filter((t) => t.completed) as task (task.id)}
					<TaskCard
						id={task.id}
						title={task.title}
						description={task.description}
						priority={task.priority}
						progress={task.progress}
						completed={true}
				onClick={() => goto(`/task/${task.id}`)}
						onToggle={() => handleToggle(task.id)}
					/>
				{/each}
			</div>
		{/if}
	{/if}
</div>

<!-- Create Modal -->
<Modal open={showCreateModal} title="Nova Tarefa" onClose={() => (showCreateModal = false)}>
	{#if createError}
		<div class="alert alert-danger" role="alert" style="margin-bottom: 1rem;">
			{createError}
		</div>
	{/if}
	<form onsubmit={(e) => { e.preventDefault(); handleCreate(); }}>
		<div style="margin-bottom: 1rem;">
			<label for="task-title" style="display: block; font-size: 0.875rem; font-weight: 500; color: var(--calm-text); margin-bottom: 0.35rem;">
				Titulo *
			</label>
			<input
				id="task-title"
				type="text"
				bind:value={newTask.title}
				placeholder="Ex: Organizar documentos..."
				required
				style="width: 100%;"
			/>
		</div>

		<div style="margin-bottom: 1rem;">
			<label for="task-description" style="display: block; font-size: 0.875rem; font-weight: 500; color: var(--calm-text); margin-bottom: 0.35rem;">
				Descricao
			</label>
			<textarea
				id="task-description"
				bind:value={newTask.description}
				placeholder="Adicione detalhes (opcional)"
				rows={3}
				style="width: 100%;"
			></textarea>
		</div>

		<div style="margin-bottom: 1.5rem;">
			<fieldset style="border: none; padding: 0; margin: 0;">
				<legend style="display: block; font-size: 0.875rem; font-weight: 500; color: var(--calm-text); margin-bottom: 0.35rem;">
					Prioridade
				</legend>
				<div style="display: flex; gap: 0.5rem; flex-wrap: wrap;" role="radiogroup" aria-label="Prioridade">
					{#each priorityOptions as opt}
						<button
							type="button"
							class="calm-btn {newTask.priority === opt.value ? 'calm-btn-primary' : 'calm-btn-secondary'}"
							onclick={() => (newTask = { ...newTask, priority: opt.value })}
							role="radio"
							aria-checked={newTask.priority === opt.value}
							style="font-size: 0.8rem; padding: 0.3rem 0.75rem;"
						>
							<PriorityBadge priority={opt.value} label={opt.label} />
						</button>
					{/each}
				</div>
			</fieldset>
		</div>

		<div style="display: flex; gap: 0.75rem; justify-content: flex-end;">
			<button
				type="button"
				class="calm-btn calm-btn-secondary"
				onclick={() => (showCreateModal = false)}
			>
				Cancelar
			</button>
			<button
				type="submit"
				class="calm-btn calm-btn-primary"
			>
				Criar Tarefa
			</button>
		</div>
	</form>
</Modal>


