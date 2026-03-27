export interface User {
  nombre: string;
  email: string;
  rol: string;
  fechaCreacion: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  email: string;
  expiresAt: string;
  fullName: string;
  role: string;
}

export interface Project {
  id: string;
  name: string;
  description: string;
  createdAt: string;
  status: ProjectStatus;
  ownerId: number;
  ownerName: string;
  tareas?: Task[];
}

export type ProjectStatus = 'Active' | 'Completed' | 'Archived' | 'Cancelled';

export interface Task {
  id: string;
  title: string;
  description: string;
  status: TaskStatus;
  createdAt: string;
  projectId: string;
  projectName: string;
  assignedToMemberId: string | null;
  assignedToName: string;
}

export type TaskStatus = 'Pending' | 'InProgress' | 'Done' | 'Cancelled';

export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

export interface PaginatedResponse<T> {
  data: T[];
  totalItems: number;
  totalPages: number;
  currentPage: number;
  pageSize: number;
}

export interface AuthContextType {
  user: User | null;
  token: string | null;
  isLoading: boolean;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
}

export interface Comment {
  id: string;
  content: string;
  userName: string;
  createdAt: string;
}

export interface ProjectMember {
  id: string;
  userId: string;
  fullName: string;
  email: string;
  role: string;
  joinedAt: string;
}

export interface AssignableUser {
  id: string;
  fullName: string;
  email: string;
  role: string;
}

export interface UserDTO {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  role: string;
  createdAt: string;
}

export interface CreateUserDTO {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  role: string;
}

export interface UpdateUserDTO {
  firstName: string;
  lastName: string;
}

