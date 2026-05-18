import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/svelte';
import { ProgressBar } from '$lib/components';

describe('ProgressBar', () => {
	it('renders with default props (0% progress)', () => {
		const { container } = render(ProgressBar, { value: 0 });
		const bar = container.querySelector('.calm-progress-bar');
		expect(bar).toBeTruthy();
	});

	it('shows correct percentage text', () => {
		render(ProgressBar, { value: 75, max: 100 });
		const label = screen.getByRole('progressbar');
		expect(label).toHaveAttribute('aria-valuenow', '75');
		expect(label).toHaveAttribute('aria-valuemax', '100');
		expect(screen.getByText('75 / 100 (75%)')).toBeTruthy();
	});

	it('bar width changes with progress value', () => {
		const { container } = render(ProgressBar, { value: 50, max: 100 });
		const bar = container.querySelector('.calm-progress-bar') as HTMLElement;
		expect(bar.style.width).toBe('50%');
	});

	it('bar width is clamped between 0% and 100%', () => {
		const { container } = render(ProgressBar, { value: -10, max: 100 });
		const bar = container.querySelector('.calm-progress-bar') as HTMLElement;
		expect(bar.style.width).toBe('0%');
	});

	it('bar width is clamped at 100% for values exceeding max', () => {
		const { container } = render(ProgressBar, { value: 150, max: 100 });
		const bar = container.querySelector('.calm-progress-bar') as HTMLElement;
		expect(bar.style.width).toBe('100%');
	});

	it('shows different color classes based on progress level', () => {
		const { container: primary } = render(ProgressBar, { value: 50, variant: 'primary' });
		const { container: success } = render(ProgressBar, { value: 50, variant: 'success' });
		const { container: warning } = render(ProgressBar, { value: 50, variant: 'warning' });

		const primaryBar = primary.querySelector('.calm-progress-bar') as HTMLElement;
		const successBar = success.querySelector('.calm-progress-bar') as HTMLElement;
		const warningBar = warning.querySelector('.calm-progress-bar') as HTMLElement;

		expect(primaryBar.className).toContain('calm-progress-bar-primary');
		expect(successBar.className).toContain('calm-progress-bar-success');
		expect(warningBar.className).toContain('calm-progress-bar-warning');
	});

	it('hides label when showLabel is false', () => {
		const { container } = render(ProgressBar, { value: 50, showLabel: false });
		expect(container.querySelector('.calm-progress-label')).toBeNull();
	});

	it('respects custom height prop', () => {
		const { container } = render(ProgressBar, { value: 50, height: 20 });
		const wrapper = container.querySelector('.calm-progress') as HTMLElement;
		expect(wrapper.style.height).toBe('20px');
	});

	it('handles micro-steps display when provided', () => {
		render(ProgressBar, { value: 3, max: 10 });
		expect(screen.getByText('3 / 10 (30%)')).toBeTruthy();
	});

	it('rounds percentage correctly for non-integer values', () => {
		render(ProgressBar, { value: 1, max: 3 });
		expect(screen.getByText('1 / 3 (33%)')).toBeTruthy();
	});
});
