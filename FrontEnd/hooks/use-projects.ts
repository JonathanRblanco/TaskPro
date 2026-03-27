import useSWR from 'swr';
import { projectsService } from '@/lib/api';
import { Project } from '@/types';
import { useAuth } from '@/contexts/auth-context';

const fetchProjects = async (): Promise<Project[]> => {
  return projectsService.getAll();
};

const fetchMyProjects = async (): Promise<Project[]> => {
  return projectsService.getMyProjects();
};

const fetchProjectById = async (id: string): Promise<Project> => {
  return projectsService.getById(id);
};


export function useMyProjects() {
    const { user } = useAuth(); 
  const isAdmin = user?.rol === 'Admin';
  const { data, error, isLoading, mutate } = useSWR<Project[]>(
    'my-projects',
    isAdmin ? fetchProjects : fetchMyProjects,
    {
      revalidateOnFocus: false,
      dedupingInterval: 500000,
    }
  );

  return {
    projects: data || [],
    isLoading,
    isError: !!error,
    error,
    mutate,
  };
}


export function useProject(id: string | null) {
  const { data, error, isLoading, mutate } = useSWR<Project>(
    id ? `project-${id}` : null,
    () => (id ? fetchProjectById(id) : Promise.reject('No ID provided')),
    {
      revalidateOnFocus: false,
      dedupingInterval: 5000,
    }
  );

  return {
    project: data,
    isLoading,
    isError: !!error,
    error,
    mutate,
  };
}
