import { describe, it, expect, beforeEach, vi } from 'vitest';
import { get } from 'svelte/store';
import { tasks, loading, addTask, updateTask, deleteTask, fetchTasks } from '$lib/stores/tasks';
import * as api from '$lib/api';

vi.mock('$lib/api', () => ({
	tasksApi: {
		list: vi.fn(),
		create: vi.fn(),
		update: vi.fn(),
		delete: vi.fn()
	}
}));

function tick() {
	return new Promise(resolve => setTimeout(resolve, 0));
}

describe('tasks store', () => {
	beforeEach(() => {
		tasks.set([]);
		loading.set(false);
		vi.clearAllMocks();
	});

	it('initial state has empty tasks array', () => {
		expect(get(tasks)).toEqual([]);
	});

	it('addTask adds a task to the store', async () => {
		vi.mocked(api.tasksApi.create).mockResolvedValue({
			id: 'test-1',
			title: 'Test task',
			description: 'A test task',
			priority: 'normal',
			status: 'Pending',
			orderIndex: 1,
			progress: 0,
			createdAt: new Date().toISOString(),
			updatedAt: new Date().toISOString(),
			microSteps: []
		});

		await addTask({
			title: 'Test task',
			description: 'A test task',
			done: false,
			priority: 'normal'
		});

		await tick();
		const currentTasks = get(tasks);
		expect(currentTasks).toHaveLength(1);
		expect(currentTasks[0].title).toBe('Test task');
		expect(currentTasks[0].done).toBe(false);
	});

	it('updateTask updates an existing task', async () => {
		vi.mocked(api.tasksApi.create).mockResolvedValue({
			id: 'test-1',
			title: 'Original',
			description: 'Desc',
			priority: 'low',
			status: 'Pending',
			orderIndex: 1,
			progress: 0,
			createdAt: new Date().toISOString(),
			updatedAt: new Date().toISOString(),
			microSteps: []
		});

		await addTask({ title: 'Original', description: 'Desc', done: false, priority: 'low' });
		await tick();
		const taskId = get(tasks)[0].id;

		vi.mocked(api.tasksApi.update).mockResolvedValue({
			id: taskId,
			title: 'Updated',
			description: 'Desc',
			priority: 'low',
			status: 'Completed',
			orderIndex: 1,
			progress: 100,
			createdAt: new Date().toISOString(),
			updatedAt: new Date().toISOString(),
			microSteps: []
		});

		await updateTask(taskId, { title: 'Updated', done: true });
		await tick();

		const currentTasks = get(tasks);
		expect(currentTasks).toHaveLength(1);
		expect(currentTasks[0].title).toBe('Updated');
		expect(currentTasks[0].done).toBe(true);
	});

	it('deleteTask removes a task from the store', async () => {
		vi.mocked(api.tasksApi.create).mockResolvedValue({
			id: 'test-1',
			title: 'To delete',
			priority: 'low',
			status: 'Pending',
			orderIndex: 1,
			progress: 0,
			createdAt: new Date().toISOString(),
			updatedAt: new Date().toISOString(),
			microSteps: []
		});

		await addTask({ title: 'To delete', done: false, priority: 'low' });
		await tick();
		expect(get(tasks)).toHaveLength(1);

		vi.mocked(api.tasksApi.delete).mockResolvedValue(undefined);
		await deleteTask(get(tasks)[0].id);
		await tick();
		expect(get(tasks)).toHaveLength(0);
	});

	it('fetchTasks loads tasks from API', async () => {
		vi.mocked(api.tasksApi.list).mockResolvedValue([
			{
				id: '1',
				title: 'New 1',
				priority: 'important',
				status: 'Completed',
				orderIndex: 1,
				progress: 100,
				createdAt: new Date().toISOString(),
				updatedAt: new Date().toISOString(),
				microSteps: []
			},
			{
				id: '2',
				title: 'New 2',
				priority: 'normal',
				status: 'Pending',
				orderIndex: 2,
				progress: 0,
				createdAt: new Date().toISOString(),
				updatedAt: new Date().toISOString(),
				microSteps: []
			}
		]);

		await fetchTasks();
		await tick();

		const currentTasks = get(tasks);
		expect(currentTasks).toHaveLength(2);
		expect(currentTasks[0].title).toBe('New 1');
		expect(currentTasks[0].done).toBe(true);
		expect(currentTasks[1].title).toBe('New 2');
		expect(currentTasks[1].done).toBe(false);
	});

	it('setLoading updates loading state', async () => {
		expect(get(loading)).toBe(false);

		loading.set(true);
		await tick();
		expect(get(loading)).toBe(true);

		loading.set(false);
		await tick();
		expect(get(loading)).toBe(false);
	});
});


