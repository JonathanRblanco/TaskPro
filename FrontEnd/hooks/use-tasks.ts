import useSWR from 'swr';
import { tasksService } from '@/lib/api';
import { Task } from '@/types';


const fetchMyTasks = async (): Promise<Task[]> => {
  return tasksService.getMyTasks();
};

const fetchTasksByProject = async (projectId: string): Promise<Task[]> => {
  return tasksService.getByProjectId(projectId);
};

const fetchTaskById = async (id: string): Promise<Task> => {
  return tasksService.getById(id);
};


export function useMyTasks() {
  const { data, error, isLoading, mutate } = useSWR<Task[]>(
    'my-tasks',
    fetchMyTasks,
    {
      revalidateOnFocus: false,
      dedupingInterval: 5000,
    }
  );

  return {
    tasks: data || [],
    isLoading,
    isError: !!error,
    error,
    mutate,
  };
}


export function useProjectTasks(projectId: string | null) {
  const { data, error, isLoading, mutate } = useSWR<Task[]>(
    projectId ? `project-tasks-${projectId}` : null,
    () => (projectId ? fetchTasksByProject(projectId) : Promise.reject('No project ID')),
    {
      revalidateOnFocus: false,
      dedupingInterval: 5000,
    }
  );

  return {
    tasks: data || [],
    isLoading,
    isError: !!error,
    error,
    mutate,
  };
}

export function useTask(id: string | null) {
  const { data, error, isLoading, mutate } = useSWR<Task>(
    id ? `task-${id}` : null,
    () => (id ? fetchTaskById(id) : Promise.reject('No ID provided')),
    {
      revalidateOnFocus: false,
      dedupingInterval: 5000,
    }
  );

  return {
    task: data,
    isLoading,
    isError: !!error,
    error,
    mutate,
  };
}
