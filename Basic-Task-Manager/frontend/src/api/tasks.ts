import { api } from './client';

export type TaskItem = {
  id: string;
  description: string;
  isCompleted: boolean;
};

export async function fetchTasks(): Promise<TaskItem[]> {
  const { data } = await api.get<TaskItem[]>('/api/tasks');
  return data;
}

export async function addTask(description: string): Promise<TaskItem> {
  const { data } = await api.post<TaskItem>('/api/tasks', {
    description,
    isCompleted: false
  });
  return data;
}

export async function updateTask(task: TaskItem): Promise<TaskItem> {
  const { data } = await api.put<TaskItem>(`/api/tasks/${task.id}`, task);
  return data;
}

export async function deleteTask(id: string): Promise<void> {
  await api.delete(`/api/tasks/${id}`);
}
