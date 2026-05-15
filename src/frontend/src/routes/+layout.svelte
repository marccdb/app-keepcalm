<script lang="ts">
	import favicon from '$lib/assets/favicon.svg';
	import '../css/app.css';
	import { onMount } from 'svelte';
	import { theme } from '$lib/stores/theme';

	let { children } = $props();

	onMount(() => {
		document.documentElement.setAttribute('data-theme', $theme);
	});
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
</svelte:head>

{@render children()}
