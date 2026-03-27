import { AssignableUser, Project, ProjectMember } from '@/types';
import apiClient from './client';

export const projectsService = {
  async getAll(): Promise<Project[]> {
    const response = await apiClient.get<Project[]>('/projects');
    return response.data;
  },

  async getMyProjects(): Promise<Project[]> {
    const response = await apiClient.get<Project[]>('/projects/my-projects');
    return response.data;
  },

  async getById(id: string): Promise<Project> {
    const response = await apiClient.get<Project>(`/projects/${id}`);
    return response.data;
  },

  create: async (data: { name: string; description: string }) => {
      const response = await apiClient.post('/projects', data);
      return response.data;
    },
    delete: async (id: string) => {
      await apiClient.delete(`/projects/${id}`);
    },

  getMembers: async (projectId: string): Promise<ProjectMember[]> => {
    const response = await apiClient.get(`/projects/${projectId}/members`);
    return response.data;
  },
  addMember: async (projectId: string, userId: string, role: string) => {
    const response = await apiClient.post(`/projects/${projectId}/members`, { userId, role });
    return response.data;
  },
  removeMember: async (projectId: string, userId: string) => {
    await apiClient.delete(`/projects/${projectId}/members/${userId}`);
  },

  // lib/api/users.ts — agrega este método
  getAssignable: async (): Promise<AssignableUser[]> => {
    const response = await apiClient.get('/users/assignable');
    return response.data;
  },
};

export default projectsService;
