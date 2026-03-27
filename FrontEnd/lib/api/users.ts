import { AssignableUser,UserDTO,CreateUserDTO,UpdateUserDTO } from '@/types';
import { apiClient } from './client';

export const usersService = {
  async getAssignable() : Promise<AssignableUser[]> {
    const response = await apiClient.get('/users/assignable');
    return response.data;
  },
  async getAll():Promise<UserDTO[]>  {
    const response = await apiClient.get('/users');
    return response.data;
  },
  async create(data: CreateUserDTO): Promise<UserDTO> {
    const response = await apiClient.post('/users', data);
    return response.data;
  },
  async update(id: string, data: UpdateUserDTO): Promise<UserDTO> {
    const response = await apiClient.put(`/users/${id}`, data);
    return response.data;
  },
  async delete(id: string): Promise<void> {
    await apiClient.delete(`/users/${id}`);
  },
};

export default usersService