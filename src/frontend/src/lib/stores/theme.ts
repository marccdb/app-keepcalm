import { writable } from 'svelte/store';

function getInitialTheme(): string {
	if (typeof window !== 'undefined' && localStorage.getItem('keepcalm-theme')) {
		return localStorage.getItem('keepcalm-theme')!;
	}
	if (typeof window !== 'undefined' && window.matchMedia('(prefers-color-scheme: dark)').matches) {
		return 'dark';
	}
	return 'light';
}

function applyTheme(theme: string) {
	if (typeof document !== 'undefined') {
		document.documentElement.setAttribute('data-theme', theme);
	}
}

export const theme = writable<string>(getInitialTheme());

theme.subscribe((value) => {
	applyTheme(value);
	if (typeof window !== 'undefined') {
		localStorage.setItem('keepcalm-theme', value);
	}
});
