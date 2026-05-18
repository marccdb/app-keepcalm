<script lang="ts">
	import { page } from '$app/stores';
	import { onMount } from 'svelte';
	import PriorityBadge from '$lib/components/PriorityBadge.svelte';
	import ProgressBar from '$lib/components/ProgressBar.svelte';
	import { Modal } from '$lib/components/index.js';
	import ThemeToggle from '$lib/components/ThemeToggle.svelte';
	import { API_URL, microStepsApi, type BackendMicroStep } from '$lib/api';

	let id = $derived($page.params.id);

	interface TaskItem {
		id: string;
		title: string;
		description: string;
		priority: 'urgent' | 'important' | 'normal' | 'low';
		progress: number;
		completed: boolean;
		createdAt: string;
		microSteps: MicroStep[];
	}

	interface MicroStep {
		id: string;
		description: string;
		completed: boolean;
	}

	let task = $state<TaskItem | null>(null);
	let loading = $state(true);
	let showStepModal = $state(false);
	let newStep = $state({ description: '' });
	let error = $state<string | null>(null);

	onMount(async () => {
		try {
			const res = await fetch(`${API_URL}/api/tasks/${id}`);
			if (!res.ok) {
				task = null;
			} else {
				task = await res.json();
				await loadMicroSteps();
			}
		} catch (e) {
			error = e instanceof Error ? e.message : 'Erro ao carregar tarefa';
			task = null;
		} finally {
			loading = false;
		}
	});

	async function loadMicroSteps() {
		try {
			const backendSteps = await microStepsApi.list(id);
			if (task) {
				task = {
					...task,
					microSteps: backendSteps.map((ms: BackendMicroStep) => ({
						id: ms.id,
						description: ms.title,
						completed: ms.isCompleted
					}))
				};
			}
		} catch (e) {
			console.error('Failed to load micro-steps:', e);
		}
	}

	async function handleAddStep() {
		if (!newStep.description.trim() || !task) return;

		try {
			const backendStep = await microStepsApi.create(id, {
				title: newStep.description,
				description: newStep.description
			});

			task = {
				...task,
				microSteps: [...task.microSteps, {
					id: backendStep.id,
					description: backendStep.title,
					completed: backendStep.isCompleted
				}]
			};
			newStep = { description: '' };
			showStepModal = false;
		} catch (e) {
			error = e instanceof Error ? e.message : 'Erro ao adicionar micro-passo';
		}
	}

	async function toggleStep(stepId: string) {
		if (!task) return;

		const step = task.microSteps.find((s) => s.id === stepId);
		if (!step) return;

		try {
			await microStepsApi.update(stepId, {
				isCompleted: !step.completed
			});

			task = {
				...task,
				microSteps: task.microSteps.map((s) =>
					s.id === stepId ? { ...s, completed: !s.completed } : s
				)
			};
		} catch (e) {
			error = e instanceof Error ? e.message : 'Erro ao atualizar micro-passo';
		}
	}

	function getProgress(): number {
		if (!task || task.microSteps.length === 0) return 0;
		const completed = task.microSteps.filter((s) => s.completed).length;
		return Math.round((completed / task.microSteps.length) * 100);
	}

	function formatDate(iso: string): string {
		return new Date(iso).toLocaleDateString('pt-BR', {
			day: '2-digit',
			month: '2-digit',
			year: 'numeric',
			hour: '2-digit',
			minute: '2-digit'
		});
	}
</script>

{#if loading}
	<div class="calm-container calm-section">
		<div class="calm-empty">
			<div class="calm-empty-icon calm-pulse">⏳</div>
			<p class="calm-empty-text">Carregando tarefa...</p>
		</div>
	</div>
{:else if error}
	<div class="calm-container calm-section">
		<div class="calm-empty calm-fade-in">
			<div class="calm-empty-icon">⚠️</div>
			<p class="calm-empty-text">{error}</p>
			<p class="calm-empty-hint"><a href="/">Voltar ao Dashboard</a></p>
		</div>
	</div>
{:else if !task}
	<div class="calm-container calm-section">
		<div class="calm-empty calm-fade-in">
			<div class="calm-empty-icon">🔍</div>
			<p class="calm-empty-text">Tarefa nao encontrada</p>
			<p class="calm-empty-hint"><a href="/">Voltar ao Dashboard</a></p>
		</div>
	</div>
{:else}
	<div class="calm-header">
		<div class="calm-container">
			<div style="display: flex; align-items: center; justify-content: space-between;">
				<a href="/" class="calm-nav-link">← Dashboard</a>
				<ThemeToggle />
			</div>
		</div>
	</div>

	<div class="calm-container calm-section" style="max-width: 720px;">
		<!-- Task header -->
		<div class="calm-fade-in" style="margin-bottom: 2rem;">
			<div style="display: flex; align-items: center; gap: 0.75rem; margin-bottom: 0.75rem; flex-wrap: wrap;">
				<PriorityBadge priority={task.priority} />
				{#if task.completed}
					<span style="color: var(--calm-success); font-size: 0.85rem;">✓ Concluida</span>
				{/if}
			</div>
			<h1 style="font-size: 1.8rem; font-weight: 700; color: var(--calm-text); margin: 0 0 0.5rem;">
				{task.title}
			</h1>
			{#if task.description}
				<p style="font-size: 0.95rem; color: var(--calm-text-secondary); line-height: 1.6; margin: 0 0 1rem;">
					{task.description}
				</p>
			{/if}
			<small style="color: var(--calm-text-muted);">
				Criada em {formatDate(task.createdAt)}
			</small>
		</div>

		<!-- Progress -->
		<div class="calm-fade-in" style="margin-bottom: 2rem;">
			<ProgressBar
				value={getProgress()}
				max={100}
				variant="primary"
				showLabel={true}
			/>
		</div>

		<hr class="calm-divider" />

		<!-- Micro steps section -->
		<div class="calm-fade-in">
			<div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 1rem;">
				<h2 style="font-size: 1.15rem; font-weight: 600; color: var(--calm-text); margin: 0;">
					Micro-passos ({task.microSteps.filter((s) => s.completed).length}/{task.microSteps.length})
				</h2>
				<button
					class="calm-btn calm-btn-primary"
					style="font-size: 0.8rem; padding: 0.3rem 0.75rem;"
					onclick={() => (showStepModal = true)}
				>
					+ Adicionar
				</button>
			</div>

			{#if task.microSteps.length === 0}
				<div class="calm-empty" style="padding: 2rem 1rem;">
					<p class="calm-empty-text">Nenhum micro-passo</p>
					<p class="calm-empty-hint">Adicione micro-passos para dividir esta tarefa.</p>
				</div>
			{:else}
				<ul class="calm-list">
					{#each task.microSteps as step (step.id)}
						<li class="calm-list-item">
							<input
								type="checkbox"
								checked={step.completed}
								onclick={() => toggleStep(step.id)}
								style="width: 1.1rem; height: 1.1rem; cursor: pointer; accent-color: var(--calm-primary);"
							/>
							<span
								style="flex: 1; font-size: 0.9rem; color: {step.completed ? 'var(--calm-text-muted)' : 'var(--calm-text)'}; text-decoration: {step.completed ? 'line-through' : 'none'}; transition: var(--calm-transition);"
							>
								{step.description}
							</span>
						</li>
					{/each}
				</ul>
			{/if}
		</div>
	</div>

	<!-- Add Step Modal -->
	<Modal open={showStepModal} title="Novo Micro-passo" onClose={() => (showStepModal = false)}>
		<form onsubmit={(e) => { e.preventDefault(); handleAddStep(); }}>
			<div style="margin-bottom: 1.5rem;">
				<label for="step-description" style="display: block; font-size: 0.875rem; font-weight: 500; color: var(--calm-text); margin-bottom: 0.35rem;">
					Descricao do passo *
				</label>
				<input
					id="step-description"
					type="text"
					bind:value={newStep.description}
					placeholder="Ex: Abrir o arquivo X..."
					required
					style="width: 100%;"
				/>
			</div>
			<div style="display: flex; gap: 0.75rem; justify-content: flex-end;">
				<button
					type="button"
					class="calm-btn calm-btn-secondary"
					onclick={() => (showStepModal = false)}
				>
					Cancelar
				</button>
				<button type="submit" class="calm-btn calm-btn-primary">
					Adicionar
				</button>
			</div>
		</form>
	</Modal>
{/if}
