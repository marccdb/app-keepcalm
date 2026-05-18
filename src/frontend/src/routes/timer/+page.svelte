<script lang="ts">
	import { onMount, onDestroy } from 'svelte';
	import ThemeToggle from '$lib/components/ThemeToggle.svelte';
	import { focusSessionsApi } from '$lib/api';

	let running = $state(false);
	let paused = $state(false);
	let timeLeft = $state(25 * 60);
	let totalTime = $state(25 * 60);
	let sessions = $state(0);
	let intervalId = 0;
	let sessionId = $state<string | null>(null);
	let error = $state<string | null>(null);

	const presets = [
		{ label: 'Foco', minutes: 25, type: 'focus' },
		{ label: 'Curta pausa', minutes: 5, type: 'break' },
		{ label: 'Longa pausa', minutes: 15, type: 'break' }
	];

	let selectedPreset = $state(presets[0]);

	async function loadSessionCount() {
		try {
			const history = await focusSessionsApi.getHistory(30);
			sessions = history.filter(s => s.status === 'completed').length;
		} catch {
			// silently fail, keep current count
		}
	}

	async function startTimer() {
		try {
			const backendSession = await focusSessionsApi.start({
				durationMinutes: selectedPreset.minutes,
				type: selectedPreset.type
			});

			sessionId = backendSession.id;
			running = true;
			paused = false;
			timeLeft = selectedPreset.minutes * 60;
			totalTime = selectedPreset.minutes * 60;
			intervalId = window.setInterval(tick, 1000);
		} catch (e) {
			error = e instanceof Error ? e.message : 'Erro ao iniciar timer';
		}
	}

	async function pauseTimer() {
		if (sessionId) {
			try {
				await focusSessionsApi.pause(sessionId);
			} catch (e) {
				console.error('Failed to pause session:', e);
			}
		}
		paused = true;
		window.clearInterval(intervalId);
	}

	async function resumeTimer() {
		if (sessionId) {
			try {
				await focusSessionsApi.resume(sessionId);
			} catch (e) {
				console.error('Failed to resume session:', e);
			}
		}
		paused = false;
		intervalId = window.setInterval(tick, 1000);
	}

	async function completeTimer() {
		window.clearInterval(intervalId);
		running = false;
		paused = false;

		if (sessionId) {
			try {
				await focusSessionsApi.complete(sessionId);
				sessions++;
			} catch (e) {
				console.error('Failed to complete session:', e);
			}
		}

		sessionId = null;
		timeLeft = selectedPreset.minutes * 60;
		totalTime = selectedPreset.minutes * 60;
	}

	function resetTimer() {
		if (sessionId) {
			sessionId = null;
		}
		running = false;
		paused = false;
		window.clearInterval(intervalId);
		timeLeft = selectedPreset.minutes * 60;
		totalTime = selectedPreset.minutes * 60;
	}

	function tick() {
		if (timeLeft > 0) {
			timeLeft--;
		} else {
			completeTimer();
		}
	}

	function selectPreset(preset: (typeof presets)[number]) {
		selectedPreset = preset;
		if (running) {
			window.clearInterval(intervalId);
			timeLeft = preset.minutes * 60;
			totalTime = preset.minutes * 60;
		}
	}

	function formatTime(seconds: number): string {
		const m = Math.floor(seconds / 60);
		const s = seconds % 60;
		return `${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
	}

	onMount(() => {
		loadSessionCount();
	});

	onDestroy(() => {
		window.clearInterval(intervalId);
	});
</script>

<div class="calm-header">
	<div class="calm-container">
		<div class="calm-nav" style="justify-content: space-between;">
			<a href="/" style="display: flex; align-items: center; gap: 0.75rem; text-decoration: none; color: var(--calm-text);">
				<span style="font-size: 1.4rem; color: var(--calm-primary);">✦</span>
				<span style="font-weight: 600;">KeepCalm</span>
			</a>
			<div style="display: flex; align-items: center; gap: 0.5rem;">
				<nav style="display: flex; gap: 0.25rem;">
					<a href="/" class="calm-nav-link">Dashboard</a>
					<a href="/timer" class="calm-nav-link active">Timer</a>
					<a href="/settings" class="calm-nav-link">Configuracoes</a>
				</nav>
				<ThemeToggle />
			</div>
		</div>
	</div>
</div>

{#if error}
	<div class="calm-container" style="margin-top: 1rem;">
		<div style="background: var(--calm-error-bg); color: var(--calm-error); padding: 0.75rem 1rem; border-radius: 8px; font-size: 0.85rem;">
			{error}
		</div>
	</div>
{/if}

<div class="calm-container calm-section" style="display: flex; flex-direction: column; align-items: center; justify-content: center; min-height: calc(100vh - 120px);">
	<!-- Title -->
	<div style="text-align: center; margin-bottom: 2rem;">
		<h1 style="font-size: 1.8rem; font-weight: 700; color: var(--calm-text); margin: 0 0 0.5rem;">
			Timer de Foco
		</h1>
		<p style="font-size: 0.95rem; color: var(--calm-text-secondary); margin: 0;">
			{sessions} sessoes completadas
		</p>
	</div>

	<!-- Preset selector -->
	<div style="display: flex; gap: 0.5rem; margin-bottom: 2rem;">
		{#each presets as preset}
			<button
				type="button"
				class="calm-btn {selectedPreset === preset ? 'calm-btn-primary' : 'calm-btn-secondary'}"
				onclick={() => selectPreset(preset)}
				style="font-size: 0.85rem; padding: 0.35rem 0.85rem;"
			>
				{preset.label}
			</button>
		{/each}
	</div>

	<!-- Timer display -->
	<div style="margin-bottom: 2rem;">
		<div class="calm-breathe" style="display: flex; align-items: center; justify-content: center;">
			<span
				class="calm-fade-in"
				style="font-size: 5rem; font-weight: 300; font-family: var(--calm-font-mono); color: {paused ? 'var(--calm-warning)' : 'var(--calm-primary-dark)'}; letter-spacing: 0.05em;"
			>
				{formatTime(timeLeft)}
			</span>
		</div>
		<p style="text-align: center; color: var(--calm-text-secondary); font-size: 0.9rem; margin-top: 0.5rem;">
			{running ? (paused ? 'Pausado' : 'Em progresso...') : selectedPreset.label}
		</p>
	</div>

	<!-- Controls -->
	<div style="display: flex; gap: 0.75rem;">
		{#if running && !paused}
			<button
				class="calm-btn calm-btn-secondary calm-btn-lg"
				onclick={pauseTimer}
			>
				⏸ Pausar
			</button>
		{:else if paused}
			<button
				class="calm-btn calm-btn-primary calm-btn-lg"
				onclick={resumeTimer}
			>
				▶ Retomar
			</button>
		{:else}
			<button
				class="calm-btn calm-btn-primary calm-btn-lg"
				onclick={startTimer}
			>
				▶ Iniciar
			</button>
		{/if}

		{#if running}
			<button
				class="calm-btn calm-btn-secondary calm-btn-lg"
				onclick={resetTimer}
			>
				↺ Reset
			</button>
		{/if}
	</div>

	<!-- Progress circle -->
	{#if running}
		<div style="margin-top: 2.5rem;">
			<div class="calm-progress" style="width: 200px; height: 6px;">
				<div
					class="calm-progress-bar calm-progress-bar-primary"
					style="width: {((totalTime - timeLeft) / totalTime) * 100}%; height: 100%; transition: width 0.5s ease;"
				></div>
			</div>
			<small style="color: var(--calm-text-muted); font-size: 0.75rem;">
				{Math.round(((totalTime - timeLeft) / totalTime) * 100)}% completo
			</small>
		</div>
	{/if}
</div>
