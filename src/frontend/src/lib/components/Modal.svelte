<script lang="ts">
	interface Props {
		open: boolean;
		title: string;
		onClose: () => void;
		children?: () => any;
		footer?: () => any;
	}

	const { open, title, onClose, children = () => {}, footer = () => {} }: Props = $props();

	$effect(() => {
		if (open) {
			document.body.style.overflow = 'hidden';
		} else {
			document.body.style.overflow = '';
		}
	});

	function handleBackdropClick(e: MouseEvent) {
		if (e.target === e.currentTarget) {
			onClose();
		}
	}
</script>

{#if open}
	<div
		class="calm-modal-backdrop"
		role="presentation"
		onclick={handleBackdropClick}
		style="position: fixed; inset: 0; background: rgba(0,0,0,0.3); display: flex; align-items: center; justify-content: center; z-index: 1050; animation: calm-fade-in 0.2s ease;"
		tabindex="-1"
	>
		<div
			class="calm-modal-content"
			role="dialog"
			aria-modal="true"
			aria-label={title}
			tabindex="0"
			style="background: var(--calm-bg-secondary); border-radius: var(--calm-radius-lg); box-shadow: var(--calm-shadow-lg); width: 100%; max-width: 520px; max-height: 90vh; overflow: auto; animation: calm-fade-in 0.2s ease;"
			onkeydown={(e) => {
				if (e.key === 'Escape') onClose();
			}}
		>
			<!-- Header -->
			<div style="display: flex; align-items: center; justify-content: space-between; padding: 1.25rem 1.5rem 0.75rem;">
				<h2 style="font-size: 1.2rem; font-weight: 600; margin: 0; color: var(--calm-text);">
					{title}
				</h2>
				<button
					class="calm-btn calm-btn-secondary calm-btn-sm"
					onclick={onClose}
					aria-label="Fechar"
					style="padding: 0.3rem 0.5rem; font-size: 1.1rem; line-height: 1;"
				>
					&times;
				</button>
			</div>

			<!-- Body -->
			<div style="padding: 0.75rem 1.5rem;">
				{@render children()}
			</div>

			<!-- Footer -->
			<div
				style="display: flex; align-items: center; justify-content: flex-end; gap: 0.75rem; padding: 0.75rem 1.5rem 1.25rem;"
			>
				{@render footer()}
			</div>
		</div>
	</div>
{/if}

<style>
	@keyframes calm-fade-in {
		from {
			opacity: 0;
			transform: scale(0.96);
		}
		to {
			opacity: 1;
			transform: scale(1);
		}
	}
</style>
