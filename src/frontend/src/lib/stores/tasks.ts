import { writable, type Writable } from 'svelte/store';
import { tasksApi, type BackendTask, type BackendMicroStep } from '$lib/api';

export interface Task {
	id: string;
	title: string;
	description?: string;
	done: boolean;
	priority: 'urgent' | 'important' | 'normal' | 'low';
	progress: number;
	completed: boolean;
	createdAt: string;
	microSteps: MicroStep[];
}

export interface MicroStep {
	id: string;
	description: string;
	completed: boolean;
}

function mapBackendTask(task: BackendTask): Task {
	return {
		id: task.id,
		title: task.title,
		description: task.description,
		done: task.status === 'completed',
		priority: task.priority,
		progress: task.progress,
		completed: task.status === 'completed',
		createdAt: task.createdAt,
		microSteps: task.microSteps.map((ms: BackendMicroStep) => ({
			id: ms.id,
			description: ms.title,
			completed: ms.isCompleted
		}))
	};
}

export const tasks: Writable<Task[]> = writable<Task[]>([]);
export const loading: Writable<boolean> = writable<boolean>(false);
export const error: Writable<string | null> = writable<string | null>(null);

const setTasks = (newTasks: Task[]) => {
	tasks.set(newTasks);
};

const setLoading = (newLoading: boolean) => {
	loading.set(newLoading);
};

const setError = (newError: string | null) => {
	error.set(newError);
};

export async function fetchTasks() {
	loading.set(true);
	error.set(null);
	try {
		const backendTasks = await tasksApi.list();
		setTasks(backendTasks.map(mapBackendTask));
	} catch (err) {
		error.set(err instanceof Error ? err.message : 'Failed to fetch tasks');
		setTasks([]);
	} finally {
		loading.set(false);
	}
}

export async function addTask(task: Omit<Task, 'id' | 'progress' | 'completed' | 'microSteps' | 'done'>) {
	try {
		const backendTask = await tasksApi.create({
			title: task.title,
			description: task.description,
			priority: task.priority
		});
		const mappedTask = mapBackendTask(backendTask);
		tasks.update((current) => [mappedTask, ...current]);
	} catch (err) {
		error.set(err instanceof Error ? err.message : 'Failed to create task');
	}
}

export async function updateTask(id: string, updates: Partial<Omit<Task, 'id'>>) {
	try {
		const backendUpdates: Record<string, string | string[]> = {};
		if (updates.title !== undefined) backendUpdates.title = updates.title;
		if (updates.description !== undefined) backendUpdates.description = updates.description;
		if (updates.priority !== undefined) backendUpdates.priority = updates.priority;
		if (updates.done !== undefined) backendUpdates.status = updates.done ? 'completed' : 'pending';

		const backendTask = await tasksApi.update(id, backendUpdates);
		const mappedTask = mapBackendTask(backendTask);

		tasks.update((current) =>
			current.map((t) => (t.id === id ? mappedTask : t))
		);
	} catch (err) {
		error.set(err instanceof Error ? err.message : 'Failed to update task');
	}
}

export async function deleteTask(id: string) {
	try {
		await tasksApi.delete(id);
		tasks.update((current) => current.filter((t) => t.id !== id));
	} catch (err) {
		error.set(err instanceof Error ? err.message : 'Failed to delete task');
	}
}

export async function toggleTask(id: string) {
	let currentTask: Task | undefined;
	tasks.subscribe((t) => {
		currentTask = t.find((t) => t.id === id);
	})();
	if (!currentTask) return;

	const newDone = !currentTask.done;
	try {
		const backendTask = await tasksApi.update(id, {
			status: newDone ? 'completed' : 'pending'
		});
		const mappedTask = mapBackendTask(backendTask);

		tasks.update((current) =>
			current.map((t) => (t.id === id ? mappedTask : t))
		);
	} catch (err) {
		error.set(err instanceof Error ? err.message : 'Failed to toggle task');
	}
}
