<script lang="ts">
	interface Props {
		value: number;
		max?: number;
		variant?: 'primary' | 'success' | 'warning';
		showLabel?: boolean;
		height?: number;
	}

	const {
		value,
		max = 100,
		variant = 'primary',
		showLabel = true,
		height = 8
	}: Props = $props();

	const percentage = $derived(Math.min(100, Math.max(0, (value / max) * 100)));

	const barClass = $derived(
		`calm-progress-bar calm-progress-bar-${variant}`
	);
</script>

<div class="calm-progress" style="height: {height}px" role="progressbar" aria-valuenow={value} aria-valuemin={0} aria-valuemax={max}>
	<div
		class={barClass}
		style="width: {percentage}%; height: 100%;"
	/>
</div>

{#if showLabel}
	<small class="calm-progress-label" style="color: var(--calm-text-secondary); margin-top: 0.25rem;">
		{value} / {max} ({Math.round(percentage)}%)
	</small>
{/if}
