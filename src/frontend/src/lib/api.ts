export const API_URL = import.meta.env.VITE_API_URL ?? '';

export interface BackendTask {
	id: string;
	title: string;
	description?: string;
	priority: 'urgent' | 'important' | 'normal' | 'low';
	status: 'pending' | 'inprogress' | 'completed' | 'cancelled';
	orderIndex: number;
	tags: string[];
	progress: number;
	createdAt: string;
	updatedAt?: string;
	microSteps: BackendMicroStep[];
}

export interface BackendMicroStep {
	id: string;
	taskId: string;
	title: string;
	description?: string;
	isCompleted: boolean;
	orderIndex: number;
	createdAt: string;
	updatedAt?: string;
}

export interface BackendFocusSession {
	id: string;
	durationMinutes: number;
	elapsedSeconds: number;
	status: 'idle' | 'running' | 'paused' | 'completed';
	type: 'focus' | 'break';
	taskId?: string;
	completedAt?: string;
	pausedAt?: string;
	startedAt: string;
	roundsCompleted: number;
}

export interface ApiResponse<T> {
	data?: T;
	error?: string;
	details?: string;
}

async function request<T>(url: string, options?: RequestInit): Promise<T> {
	const response = await fetch(`${API_URL}${url}`, {
		headers: {
			'Content-Type': 'application/json',
			...options?.headers
		},
		...options
	});

	if (!response.ok) {
		const error = await response.json().catch(() => ({ error: response.statusText }));
		throw new Error(error.details || error.error || `Request failed: ${response.status}`);
	}

	return response.json();
}

// Tasks API
export const tasksApi = {
	list: (priority?: string, status?: string) => {
		const params = new URLSearchParams();
		if (priority) params.set('priority', priority);
		if (status) params.set('status', status);
		const query = params.toString() ? `?${params.toString()}` : '';
		return request<BackendTask[]>(`/api/tasks${query}`);
	},

	get: (id: string) => request<BackendTask>(`/api/tasks/${id}`),

	create: (data: { title: string; description?: string; priority: string; folder?: string }) =>
		request<BackendTask>('/api/tasks', {
			method: 'POST',
			body: JSON.stringify(data)
		}),

	update: (id: string, data: { title?: string; description?: string; priority?: string; status?: string; tags?: string[] }) =>
		request<BackendTask>(`/api/tasks/${id}`, {
			method: 'PUT',
			body: JSON.stringify(data)
		}),

	delete: (id: string) =>
		request<void>(`/api/tasks/${id}`, {
			method: 'DELETE'
		}),

	reorder: (id: string, newIndex: number) =>
		request<BackendTask[]>(`/api/tasks/${id}/reorder`, {
			method: 'PATCH',
			body: JSON.stringify({ newIndex })
		})
};

// Micro-steps API
export const microStepsApi = {
	list: (taskId: string) => request<BackendMicroStep[]>(`/api/micro-steps/task/${taskId}`),

	create: (taskId: string, data: { title: string; description?: string }) =>
		request<BackendMicroStep>(`/api/micro-steps/task/${taskId}`, {
			method: 'POST',
			body: JSON.stringify(data)
		}),

	update: (id: string, data: { title?: string; description?: string; isCompleted?: boolean }) =>
		request<BackendMicroStep>(`/api/micro-steps/${id}`, {
			method: 'PATCH',
			body: JSON.stringify(data)
		}),

	delete: (id: string) =>
		request<void>(`/api/micro-steps/${id}`, {
			method: 'DELETE'
		}),

	deleteByTask: (taskId: string) =>
		request<void>(`/api/micro-steps/task/${taskId}`, {
			method: 'DELETE'
		})
};

// Focus Sessions API
export const focusSessionsApi = {
	start: (data: { durationMinutes: number; type: string; taskId?: string }) =>
		request<BackendFocusSession>('/api/focus-sessions/start', {
			method: 'POST',
			body: JSON.stringify(data)
		}),

	pause: (id: string) =>
		request<BackendFocusSession>(`/api/focus-sessions/${id}/pause`, {
			method: 'PATCH'
		}),

	resume: (id: string) =>
		request<BackendFocusSession>(`/api/focus-sessions/${id}/resume`, {
			method: 'PATCH'
		}),

	complete: (id: string) =>
		request<BackendFocusSession>(`/api/focus-sessions/${id}/complete`, {
			method: 'POST'
		}),

	getHistory: (days = 30) =>
		request<BackendFocusSession[]>(`/api/focus-sessions?days=${days}`)
};
