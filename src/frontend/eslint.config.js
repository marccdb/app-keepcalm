import js from '@eslint/js';
import ts from 'typescript-eslint';
import svelte from 'svelte-eslint-parser';
import prettier from 'eslint-plugin-prettier/recommended';

export default [
	js.configs.recommended,
	...ts.configs.recommended,
	prettier,
	{
		files: ['**/*.svelte'],
		languageOptions: {
			parser: svelte.parser,
			parserOptions: {
				parser: ts.parser
			}
		}
	},
	{
		files: ['**/*.ts', '**/*.js'],
		languageOptions: {
			parser: ts.parser
		}
	},
	{
		rules: {
			'@typescript-eslint/no-unused-vars': [
				'warn',
				{ argsIgnorePattern: '^_', varsIgnorePattern: '^_' }
			],
			'@typescript-eslint/ban-ts-comment': 'off',
			'prettier/prettier': 'warn'
		}
	}
];
