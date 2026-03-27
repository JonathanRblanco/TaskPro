import { Comment } from '@/types';
import apiClient from './client';

export const commentsService = {
  async getByTask(taskId: string): Promise<Comment[]>{
    const response = await apiClient.get(`/tasks/${taskId}/comments`);
    return response.data;
  },
  async create(taskId: string, content: string): Promise<Comment>{
  const response = await apiClient.post(`/tasks/${taskId}/comments`, { content });
  return response.data;
},
};

export default commentsService;