import { defineConfig } from 'vitest/config';
import { svelte, vitePreprocess } from '@sveltejs/vite-plugin-svelte';
import { sveltekit } from '@sveltejs/kit/vite';

export default defineConfig({
	plugins: [
		sveltekit(),
		svelte({
			preprocess: vitePreprocess(),
			compilerOptions: {
				runes: ({ filename }) => !filename.includes('node_modules')
			}
		})
	],
	test: {
		globals: true,
		environment: 'jsdom',
		setupFiles: ['tests/setup.ts'],
		reporters: 'default'
	}
});
