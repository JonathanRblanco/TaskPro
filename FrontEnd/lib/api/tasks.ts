import { Task } from '@/types';
import apiClient from './client';

export const tasksService = {


  async getMyTasks(): Promise<Task[]> {
    const response = await apiClient.get<Task[]>('/tasks/my-tasks');
    return response.data;
  },

  async getByProjectId(projectId: string): Promise<Task[]> {
    const response = await apiClient.get<Task[]>(`/tasks?projectId=${projectId}`);
    return response.data;
  },

  async getById(id: string): Promise<Task> {
    const response = await apiClient.get<Task>(`/tareas/${id}`);
    return response.data;
  },

  async create (data: {
    title: string;
    description: string;
    projectId: string;
    dueDate?: string;
  }): Promise<Task> {
    const response = await apiClient.post('/tasks', data);
    return response.data;
  },
  async update(id: number, task: Partial<Task>): Promise<Task> {
    const response = await apiClient.put<Task>(`/tareas/${id}`, task);
    return response.data;
  },

  async delete(id: string): Promise<void> {
    await apiClient.delete(`/tasks/${id}`);
  },

  async updateStatus(id: string, status: Task['status']): Promise<Task> {
    const response = await apiClient.patch<Task>(`/tasks/${id}/status`, { status });
    return response.data;
  },
  async assign (taskId: string, userId: string): Promise<Task> {
    const response = await apiClient.patch(`/tasks/${taskId}/assign`, { userId });
    return response.data;
  },
};

export default tasksService;
