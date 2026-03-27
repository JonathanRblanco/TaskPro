import useSWR from 'swr';
import { ProjectMember } from '@/types';
import { projectsService } from '@/lib/api/projects';

export function useProjectMembers(projectId: string) {
  const { data, isLoading, mutate } = useSWR<ProjectMember[]>(
    `project-members-${projectId}`,
    () => projectsService.getMembers(projectId)
  );

  return {
    members: data || [],
    isLoading,
    mutate,
  };
}