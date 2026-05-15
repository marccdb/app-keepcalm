<script lang="ts">
	import { onMount } from 'svelte';
	import TaskCard from '$lib/components/TaskCard.svelte';
	import { Modal } from '$lib/components/index.js';
	import PriorityBadge from '$lib/components/PriorityBadge.svelte';
	import ThemeToggle from '$lib/components/ThemeToggle.svelte';
	import { API_URL } from '$lib/api';

	interface TaskItem {
		id: string;
		title: string;
		description: string;
		priority: 'low' | 'medium' | 'high' | 'urgent';
		progress: number;
		completed: boolean;
		createdAt: string;
	}

	let tasks: TaskItem[] = $state([]);
	let loading = $state(true);
	let showCreateModal = $state(false);
	let newTask = $state<{ title: string; description: string; priority: 'low' | 'medium' | 'high' | 'urgent' }>({ title: '', description: '', priority: 'medium' });

	const priorityOptions = [
		{ value: 'low', label: 'Baixa', color: 'info' },
		{ value: 'medium', label: 'Media', color: 'primary' },
		{ value: 'high', label: 'Alta', color: 'warning' },
		{ value: 'urgent', label: 'Urgente', color: 'danger' }
	] as const;

	onMount(async () => {
		try {
			const res = await fetch(`${API_URL}/api/tasks`);
			if (res.ok) {
				const data = await res.json();
				tasks = data;
			}
		} catch {
			tasks = [];
		} finally {
			loading = false;
		}
	});

	function handleCreate() {
		if (!newTask.title.trim()) return;

		const task: TaskItem = {
			id: crypto.randomUUID(),
			title: newTask.title,
			description: newTask.description,
			priority: newTask.priority,
			progress: 0,
			completed: false,
			createdAt: new Date().toISOString()
		};

		tasks = [task, ...tasks];
		newTask = { title: '', description: '', priority: 'medium' };
		showCreateModal = false;
	}

	function toggleTask(id: string) {
		tasks = tasks.map((t) => (t.id === id ? { ...t, completed: !t.completed } : t));
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
			<p style="font-size: 0.95rem; color: var(--calm-text-secondary); margin: 0;">
				{tasks.filter((t) => !t.completed).length} tarefas pendentes · {tasks.filter((t) => t.completed).length} concluidas
			</p>
		</div>
		<button class="calm-btn calm-btn-primary calm-btn-lg" onclick={() => (showCreateModal = true)}>
			+ Nova Tarefa
		</button>
	</div>

	<!-- Loading state -->
	{#if loading}
		<div class="calm-empty">
			<div class="calm-empty-icon calm-pulse">⏳</div>
			<p class="calm-empty-text">Carregando tarefas...</p>
		</div>
	{:else if tasks.length === 0}
		<!-- Empty state -->
		<div class="calm-empty calm-fade-in">
			<div class="calm-empty-icon">🌿</div>
			<p class="calm-empty-text">Nenhuma tarefa ainda</p>
			<p class="calm-empty-hint">Clique em "Nova Tarefa" para começar.</p>
		</div>
	{:else}
		<!-- Active tasks -->
		{#if tasks.filter((t) => !t.completed).length > 0}
			<h2 style="font-size: 1.15rem; font-weight: 600; color: var(--calm-text); margin-bottom: 1rem;">
				Pendentes
			</h2>
			<div style="display: flex; flex-direction: column; gap: 0.75rem; margin-bottom: 2rem;">
		{#each tasks.filter((t) => !t.completed) as task}
				<TaskCard
					id={task.id}
					title={task.title}
					description={task.description}
					priority={task.priority}
					progress={task.progress}
					onClick={() => window.location.href = `/task/${task.id}`}
					onToggle={() => toggleTask(task.id)}
				/>
			{/each}
			</div>
		{/if}

		<!-- Completed tasks -->
		{#if tasks.filter((t) => t.completed).length > 0}
			<h2 style="font-size: 1.15rem; font-weight: 600; color: var(--calm-text); margin-bottom: 1rem;">
				Concluidas
			</h2>
			<div style="display: flex; flex-direction: column; gap: 0.75rem;">
				{#each tasks.filter((t) => t.completed) as task}
					<TaskCard
						id={task.id}
						title={task.title}
						description={task.description}
						priority={task.priority}
						progress={task.progress}
						completed={true}
						onClick={() => window.location.href = `/task/${task.id}`}
						onToggle={() => toggleTask(task.id)}
					/>
				{/each}
			</div>
		{/if}
	{/if}
</div>

<!-- Create Modal -->
<Modal open={showCreateModal} title="Nova Tarefa" onClose={() => (showCreateModal = false)}>
	<form onsubmit={(e) => { e.preventDefault(); handleCreate(); }}>
		<div style="margin-bottom: 1rem;">
			<label style="display: block; font-size: 0.875rem; font-weight: 500; color: var(--calm-text); margin-bottom: 0.35rem;">
				Titulo *
			</label>
			<input
				type="text"
				bind:value={newTask.title}
				placeholder="Ex: Organizar documentos..."
				required
				style="width: 100%;"
			/>
		</div>

		<div style="margin-bottom: 1rem;">
			<label style="display: block; font-size: 0.875rem; font-weight: 500; color: var(--calm-text); margin-bottom: 0.35rem;">
				Descricao
			</label>
			<textarea
				bind:value={newTask.description}
				placeholder="Adicione detalhes (opcional)"
				rows={3}
				style="width: 100%;"
			/>
		</div>

		<div style="margin-bottom: 1.5rem;">
			<label style="display: block; font-size: 0.875rem; font-weight: 500; color: var(--calm-text); margin-bottom: 0.35rem;">
				Prioridade
			</label>
			<div style="display: flex; gap: 0.5rem; flex-wrap: wrap;">
				{#each priorityOptions as opt}
					<button
						type="button"
						class="calm-btn {newTask.priority === opt.value ? 'calm-btn-primary' : 'calm-btn-secondary'}"
						onclick={() => (newTask = { ...newTask, priority: opt.value })}
						style="font-size: 0.8rem; padding: 0.3rem 0.75rem;"
					>
						<PriorityBadge priority={opt.value} label={opt.label} />
					</button>
				{/each}
			</div>
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
