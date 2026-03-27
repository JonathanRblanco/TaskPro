import { LoginRequest, LoginResponse, User } from '@/types';
import apiClient from './client';

export const authService = {

  async login(credentials: LoginRequest): Promise<LoginResponse> {
    const response = await apiClient.post<LoginResponse>('/auth/login', credentials);
    return response.data;
  },

  async getCurrentUser(): Promise<User> {
    const response = await apiClient.get<User>('/auth/me');
    return response.data;
  },


  async logout(): Promise<void> {
    try {
      await apiClient.post('/auth/logout');
    } catch {
    }
  },

  async refreshToken(): Promise<{ token: string }> {
    const response = await apiClient.post<{ token: string }>('/auth/refresh');
    return response.data;
  },
};

export default authService;
