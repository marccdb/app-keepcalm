<script lang="ts">
	import ThemeToggle from '$lib/components/ThemeToggle.svelte';
	import { theme } from '$lib/stores/theme';

	let timerMinutes = $state(25);
	let shortBreakMinutes = $state(5);
	let longBreakMinutes = $state(15);
	let sessionsBeforeLongBreak = $state(4);

	function handleSave() {
		localStorage.setItem('keepcalm-settings', JSON.stringify({
			timerMinutes,
			shortBreakMinutes,
			longBreakMinutes,
			sessionsBeforeLongBreak
		}));
		alert('Configuracoes salvas!');
	}

	$effect(() => {
		const saved = localStorage.getItem('keepcalm-settings');
		if (saved) {
			const settings = JSON.parse(saved);
			timerMinutes = settings.timerMinutes ?? 25;
			shortBreakMinutes = settings.shortBreakMinutes ?? 5;
			longBreakMinutes = settings.longBreakMinutes ?? 15;
			sessionsBeforeLongBreak = settings.sessionsBeforeLongBreak ?? 4;
		}
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
					<a href="/timer" class="calm-nav-link">Timer</a>
					<a href="/settings" class="calm-nav-link active">Configuracoes</a>
				</nav>
				<ThemeToggle />
			</div>
		</div>
	</div>
</div>

<div class="calm-container calm-section" style="max-width: 600px;">
	<h1 style="font-size: 1.8rem; font-weight: 700; color: var(--calm-text); margin: 0 0 0.5rem;">
		Configuracoes
	</h1>
	<p style="font-size: 0.95rem; color: var(--calm-text-secondary); margin: 0 0 2rem;">
		Personalize o KeepCalm para o seu estilo.
	</p>

	<div class="calm-card" style="padding: 1.5rem; margin-bottom: 1.5rem;">
		<h2 style="font-size: 1.1rem; font-weight: 600; color: var(--calm-text); margin: 0 0 1.25rem;">
			Timer de Foco
		</h2>

		<div style="display: flex; flex-direction: column; gap: 1rem;">
			<div>
				<label for="timer-minutes" style="display: block; font-size: 0.875rem; font-weight: 500; color: var(--calm-text); margin-bottom: 0.35rem;">
					Sessao de foco (minutos)
				</label>
				<input
					id="timer-minutes"
					type="number"
					bind:value={timerMinutes}
					min={1}
					max={120}
					style="width: 100px;"
				/>
			</div>

			<div>
				<label for="short-break-minutes" style="display: block; font-size: 0.875rem; font-weight: 500; color: var(--calm-text); margin-bottom: 0.35rem;">
					Pausa curta (minutos)
				</label>
				<input
					id="short-break-minutes"
					type="number"
					bind:value={shortBreakMinutes}
					min={1}
					max={30}
					style="width: 100px;"
				/>
			</div>

			<div>
				<label for="long-break-minutes" style="display: block; font-size: 0.875rem; font-weight: 500; color: var(--calm-text); margin-bottom: 0.35rem;">
					Pausa longa (minutos)
				</label>
				<input
					id="long-break-minutes"
					type="number"
					bind:value={longBreakMinutes}
					min={1}
					max={60}
					style="width: 100px;"
				/>
			</div>

			<div>
				<label for="sessions-before-long-break" style="display: block; font-size: 0.875rem; font-weight: 500; color: var(--calm-text); margin-bottom: 0.35rem;">
					Sessoes antes da pausa longa
				</label>
				<input
					id="sessions-before-long-break"
					type="number"
					bind:value={sessionsBeforeLongBreak}
					min={1}
					max={10}
					style="width: 100px;"
				/>
			</div>
		</div>
	</div>

	<div class="calm-card" style="padding: 1.5rem; margin-bottom: 1.5rem;">
		<h2 style="font-size: 1.1rem; font-weight: 600; color: var(--calm-text); margin: 0 0 1.25rem;">
			Aparencia
		</h2>

		<div style="display: flex; flex-direction: column; gap: 1rem;">
			<div style="display: flex; align-items: center; justify-content: space-between;">
				<span style="font-size: 0.9rem; color: var(--calm-text);">Tema</span>
				<ThemeToggle />
			</div>
			<div style="display: flex; align-items: center; justify-content: space-between;">
				<span style="font-size: 0.9rem; color: var(--calm-text);">Mostrar animacoes de respiracao</span>
				<input id="breathing-anim" type="checkbox" checked style="width: 1.1rem; height: 1.1rem; accent-color: var(--calm-primary);" />
				<label for="breathing-anim" style="display: none;">Breathing animations</label>
			</div>
			<div style="display: flex; align-items: center; justify-content: space-between;">
				<span style="font-size: 0.9rem; color: var(--calm-text);">Som de notificacao</span>
				<input id="notification-sound" type="checkbox" checked style="width: 1.1rem; height: 1.1rem; accent-color: var(--calm-primary);" />
				<label for="notification-sound" style="display: none;">Notification sound</label>
			</div>
			<div style="display: flex; align-items: center; justify-content: space-between;">
				<span style="font-size: 0.9rem; color: var(--calm-text);">Notificacoes do navegador</span>
				<input id="browser-notifications" type="checkbox" style="width: 1.1rem; height: 1.1rem; accent-color: var(--calm-primary);" />
				<label for="browser-notifications" style="display: none;">Browser notifications</label>
			</div>
		</div>
	</div>

	<button class="calm-btn calm-btn-primary calm-btn-lg" style="width: 100%;" onclick={handleSave}>
		Salvar Configuracoes
	</button>
</div>
