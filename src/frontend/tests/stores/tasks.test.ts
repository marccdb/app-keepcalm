import { describe, it, expect, beforeEach } from 'vitest';
import { tasks } from '$lib/stores/tasks';
import type { Task } from '$lib/stores/tasks';

describe('tasks store', () => {
	beforeEach(() => {
		tasks.reset();
	});
	it('initial state has empty tasks array', () => {
		let currentTasks: Task[] = [];
		tasks.tasks.subscribe((value) => {
			currentTasks = value;
		});
		expect(currentTasks).toEqual([]);
	});

	it('addTask adds a task to the store', () => {
		let currentTasks: Task[] = [];
		tasks.tasks.subscribe((value) => {
			currentTasks = value;
		});

		tasks.addTask({
			title: 'Test task',
			description: 'A test task',
			done: false,
			priority: 'high'
		});

		expect(currentTasks).toHaveLength(1);
		expect(currentTasks[0].title).toBe('Test task');
		expect(currentTasks[0].done).toBe(false);
		expect(currentTasks[0].priority).toBe('high');
		expect(currentTasks[0].id).toBeTruthy();
	});

	it('updateTask updates an existing task', () => {
		let currentTasks: Task[] = [];
		tasks.tasks.subscribe((value) => {
			currentTasks = value;
		});

		const newTask = {
			title: 'Original',
			description: 'Desc',
			done: false,
			priority: 'low'
		};
		tasks.addTask(newTask);
		const taskId = currentTasks[0].id;

		tasks.updateTask(taskId, { title: 'Updated', done: true });

		expect(currentTasks).toHaveLength(1);
		expect(currentTasks[0].title).toBe('Updated');
		expect(currentTasks[0].done).toBe(true);
		expect(currentTasks[0].priority).toBe('low');
	});

	it('updateTask throws error for non-existent task', () => {
		expect(() => {
			tasks.updateTask('non-existent-id', { title: 'Nope' });
		}).toThrow('Task with id "non-existent-id" not found');
	});

	it('deleteTask removes a task from the store', () => {
		let currentTasks: Task[] = [];
		tasks.tasks.subscribe((value) => {
			currentTasks = value;
		});

		tasks.addTask({
			title: 'To delete',
			done: false,
			priority: 'medium'
		});
		expect(currentTasks).toHaveLength(1);

		tasks.deleteTask(currentTasks[0].id);
		expect(currentTasks).toHaveLength(0);
	});

	it('deleteTask throws error for non-existent task', () => {
		expect(() => {
			tasks.deleteTask('non-existent-id');
		}).toThrow('Task with id "non-existent-id" not found');
	});

	it('setTasks replaces all tasks', () => {
		let currentTasks: Task[] = [];
		tasks.tasks.subscribe((value) => {
			currentTasks = value;
		});

		tasks.addTask({ title: 'Old', done: false, priority: 'low' });
		expect(currentTasks).toHaveLength(1);

		const newTasks: Task[] = [
			{ id: '1', title: 'New 1', done: true, priority: 'high' },
			{ id: '2', title: 'New 2', done: false, priority: 'medium' }
		];
		tasks.setTasks(newTasks);
		expect(currentTasks).toHaveLength(2);
		expect(currentTasks[0].title).toBe('New 1');
		expect(currentTasks[1].title).toBe('New 2');
	});

	it('setLoading updates loading state', () => {
		let currentLoading = false;
		tasks.loading.subscribe((value) => {
			currentLoading = value;
		});

		expect(currentLoading).toBe(false);

		tasks.setLoading(true);
		expect(currentLoading).toBe(true);

		tasks.setLoading(false);
		expect(currentLoading).toBe(false);
	});
});
