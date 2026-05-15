<script lang="ts">
	interface Props {
		priority: 'low' | 'medium' | 'high' | 'urgent';
		label?: string;
	}

	const { priority, label }: Props = $props();

	const badgeClass = $derived(
		{
			low: 'calm-badge calm-badge-info',
			medium: 'calm-badge calm-badge-primary',
			high: 'calm-badge calm-badge-warning',
			urgent: 'calm-badge calm-badge-danger'
		}[priority]
	);

	const text = $derived(label ?? priority);
</script>

<span class={badgeClass}>{text}</span>
