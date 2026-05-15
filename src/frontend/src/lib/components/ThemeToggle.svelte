<script lang="ts">
	import { theme } from '$lib/stores/theme';

	function toggle() {
		theme.update((current) => (current === 'light' ? 'dark' : 'light'));
	}
</script>

<button
	class="theme-toggle"
	onclick={toggle}
	aria-label="Alternar tema"
	title="Alternar tema"
>
	{#if $theme === 'light'}
		☀️
	{:else}
		🌙
	{/if}
</button>

<style>
	.theme-toggle {
		display: inline-flex;
		align-items: center;
		justify-content: center;
		width: 36px;
		height: 36px;
		border-radius: var(--calm-radius-sm);
		font-size: 1.1rem;
		transition: var(--calm-transition);
		color: var(--calm-text-secondary);
	}

	.theme-toggle:hover {
		color: var(--calm-text);
		background: var(--calm-bg-tertiary);
	}
</style>
