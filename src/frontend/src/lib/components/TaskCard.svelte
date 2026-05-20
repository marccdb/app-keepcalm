<script lang="ts">
	import PriorityBadge from './PriorityBadge.svelte';
	import ProgressBar from './ProgressBar.svelte';

	let { id, title, description, priority = 'normal', progress = 0, completed = false, onClick, onToggle } = $props();

	const cardClass = $derived(
		`calm-card card-item${completed ? ' card-item--completed' : ''}`
	);

	const isChecked = $derived(completed);

	function handleKeyDown(e: KeyboardEvent) {
		if (e.key === 'Enter' || e.key === ' ') {
			e.preventDefault();
			onClick?.();
		}
	}
</script>

<button
	type="button"
	class={cardClass}
	onclick={onClick}
	onkeydown={handleKeyDown}
	tabindex="0"
	aria-label={title}
	style="padding: 1.25rem; cursor: {onClick ? 'pointer' : 'default'}; border-radius: var(--calm-radius); display: block; width: 100%; text-align: left; background: var(--calm-bg-tertiary); border: none; font-family: inherit;"
>
	<div style="display: flex; align-items: flex-start; gap: 1rem;">
		<!-- Checkbox -->
		<input
			type="checkbox"
			checked={isChecked}
			class="form-check-input"
			style="margin-top: 0.25rem; flex-shrink: 0; width: 1.1rem; height: 1.1rem; cursor: pointer; accent-color: var(--calm-primary);"
			onclick={(e) => {
				e.stopPropagation();
				onToggle?.();
			}}
		/>

		<!-- Content -->
		<div style="flex: 1; min-width: 0;">
			<div style="display: flex; align-items: center; gap: 0.75rem; margin-bottom: 0.35rem; flex-wrap: wrap;">
				<h3
					style="font-size: 1.05rem; font-weight: 600; margin: 0; color: {completed ? 'var(--calm-text-muted)' : 'var(--calm-text)'}; text-decoration: {completed ? 'line-through' : 'none'}; transition: var(--calm-transition);"
				>
					{title}
				</h3>
				<PriorityBadge priority={priority} />
			</div>

			{#if description}
				<p
					style="font-size: 0.875rem; color: var(--calm-text-secondary); margin: 0 0 0.75rem; line-height: 1.5; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden;"
				>
					{description}
				</p>
			{/if}

			<!-- Progress -->
			<div style="display: flex; align-items: center; gap: 0.75rem;">
				<div style="flex: 1;">
					<ProgressBar value={progress} max={100} variant="primary" height={6} showLabel={false} />
				</div>
				{#if completed}
					<span style="color: var(--calm-success); font-size: 0.8rem;">✓ Concluida</span>
				{/if}
			</div>
		</div>
	</div>
</button>

<style>
	.card-item {
		background: var(--calm-bg-tertiary) !important;
		color: var(--calm-text) !important;
	}

	.card-item h3 {
		color: var(--calm-text) !important;
	}

	.card-item--completed {
		opacity: 0.7;
	}
</style>
