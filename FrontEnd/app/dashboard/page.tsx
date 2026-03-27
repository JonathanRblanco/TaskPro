'use client';

import { useAuth } from '@/contexts/auth-context';
import { useMyProjects } from '@/hooks/use-projects';
import { useMyTasks } from '@/hooks/use-tasks';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import {
  FolderKanban,
  CheckSquare,
  Clock,
  AlertTriangle,
  TrendingUp,
  Check,
} from 'lucide-react';
import Link from 'next/link';
import { TaskStatus } from '@/types';

const statusLabels: Record<string, string> = {
  Completed: 'Completado',
  Active: 'En progreso',
  Archived: 'Pausado',
  Cancelled: 'Cancelado',
};
const statusTaskLabels: Record<TaskStatus, string> = {
  InProgress: 'Activo',
  Done: 'Completado',
  Pending: 'Pendiente',
  Cancelled: 'Cancelado',
};
const statusColors = {
  Pending: 'bg-yellow-500/20 text-yellow-400 border-yellow-500/30',
  InProgress: 'bg-blue-500/20 text-blue-400 border-blue-500/30',
  Done: 'bg-green-500/20 text-green-400 border-green-500/30',
  Cancelled: 'bg-red-500/20 text-red-400 border-red-500/30',
};

export default function DashboardPage() {
  const { user } = useAuth();
  const { projects, isLoading: projectsLoading } = useMyProjects();
  const { tasks, isLoading: tasksLoading } = useMyTasks();

  const pendingTasks = tasks.filter((t) => t.status === 'Pending');
  const inProgressTasks = tasks.filter((t) => t.status === 'InProgress');
  const completedTasks = tasks.filter((t) => t.status === 'Done');
  const cancelledTasks = tasks.filter((t) => t.status === 'Cancelled');

  const stats = [
    {
      title: 'Proyectos Activos',
      value: projects.filter((p) => p.status.toString() === 'Active').length,
      icon: FolderKanban,
      color: 'text-primary',
      bgColor: 'bg-primary/10',
    },
    {
      title: 'Tareas Terminadas',
      value: completedTasks.length,
      icon: Check,
      color: 'text-primary',
      bgColor: 'bg-primary/10',
    },
    {
      title: 'Tareas Pendientes',
      value: pendingTasks.length,
      icon: Clock,
      color: 'text-yellow-400',
      bgColor: 'bg-yellow-500/10',
    },
    {
      title: 'En Progreso',
      value: inProgressTasks.length,
      icon: TrendingUp,
      color: 'text-blue-400',
      bgColor: 'bg-blue-500/10',
    },
    {
      title: 'Tareas Canceladas',
      value: cancelledTasks.length,
      icon: AlertTriangle,
      color: 'text-red-400',
      bgColor: 'bg-red-500/10',
    },
  ];

  return (
    <div className="space-y-8">
      <div className="pt-12 md:pt-0">
        <h1 className="text-3xl font-bold text-foreground">
          Hola, {user?.nombre || 'Usuario'}
        </h1>
        <p className="mt-1 text-muted-foreground">
          Aqui tienes un resumen de tu trabajo
        </p>
      </div>
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {stats.map((stat) => (
          <Card key={stat.title} className="border-border/50 bg-card/50">
            <CardContent className="p-6">
              <div className="flex items-center gap-4">
                <div className={`rounded-lg p-3 ${stat.bgColor}`}>
                  <stat.icon className={`h-6 w-6 ${stat.color}`} />
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">{stat.title}</p>
                  {projectsLoading || tasksLoading ? (
                    <Skeleton className="h-8 w-12 mt-1" />
                  ) : (
                    <p className="text-2xl font-bold text-foreground">{stat.value}</p>
                  )}
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        <Card className="border-border/50 bg-card/50">
          <CardHeader className="flex flex-row items-center justify-between">
            <div>
              <CardTitle className="text-lg font-semibold text-foreground">
                Proyectos Recientes
              </CardTitle>
              <CardDescription>Tus proyectos asignados</CardDescription>
            </div>
            <Link
              href="/dashboard/projects"
              className="text-sm text-primary hover:underline"
            >
              Ver todos
            </Link>
          </CardHeader>
          <CardContent>
            {projectsLoading ? (
              <div className="space-y-4">
                {[1, 2, 3].map((i) => (
                  <Skeleton key={i} className="h-16 w-full" />
                ))}
              </div>
            ) : projects.length === 0 ? (
              <div className="flex flex-col items-center justify-center py-8 text-center">
                <FolderKanban className="h-12 w-12 text-muted-foreground/50" />
                <p className="mt-2 text-sm text-muted-foreground">
                  No tienes proyectos asignados
                </p>
              </div>
            ) : (
              <div className="space-y-3">
                {projects.slice(0, 4).map((project) => (
                  <Link
                    key={project.id}
                    href={`/dashboard/projects/${project.id}`}
                    className="flex items-center justify-between rounded-lg border border-border/50 bg-secondary/30 p-4 transition-colors hover:bg-secondary/50"
                  >
                    <div className="min-w-0 flex-1">
                      <p className="truncate font-medium text-foreground">
                        {project.name}
                      </p>
                      <p className="truncate text-sm text-muted-foreground">
                        {project.description}
                      </p>
                    </div>
                    <Badge
                      variant="outline"
                      className={
                        project.status === 'Active'
                          ? 'border-green-500/30 bg-green-500/20 text-green-400'
                          : project.status === 'Completed'
                          ? 'border-blue-500/30 bg-blue-500/20 text-blue-400'
                          : 'border-yellow-500/30 bg-yellow-500/20 text-yellow-400'
                      }
                    >
                      {statusLabels[project.status]}
                    </Badge>
                  </Link>
                ))}
              </div>
            )}
          </CardContent>
        </Card>
        <Card className="border-border/50 bg-card/50">
          <CardHeader className="flex flex-row items-center justify-between">
            <div>
              <CardTitle className="text-lg font-semibold text-foreground">
                Tareas Recientes
              </CardTitle>
              <CardDescription>Tus tareas asignadas</CardDescription>
            </div>
            <Link
              href="/dashboard/tasks"
              className="text-sm text-primary hover:underline"
            >
              Ver todas
            </Link>
          </CardHeader>
          <CardContent>
            {tasksLoading ? (
              <div className="space-y-4">
                {[1, 2, 3].map((i) => (
                  <Skeleton key={i} className="h-16 w-full" />
                ))}
              </div>
            ) : tasks.length === 0 ? (
              <div className="flex flex-col items-center justify-center py-8 text-center">
                <CheckSquare className="h-12 w-12 text-muted-foreground/50" />
                <p className="mt-2 text-sm text-muted-foreground">
                  No tienes tareas asignadas
                </p>
              </div>
            ) : (
              <div className="space-y-3">
                {tasks.slice(0, 4).map((task) => (
                  <div
                    key={task.id}
                    className="flex items-center justify-between rounded-lg border border-border/50 bg-secondary/30 p-4"
                  >
                    <div className="min-w-0 flex-1">
                      <p className="truncate font-medium text-foreground">
                        {task.title}
                      </p>
                      <p className="truncate text-sm text-muted-foreground">
                        {task.description}
                      </p>
                    </div>
                    <div className="flex flex-col items-end gap-1">
                      <Badge
                        variant="outline"
                        className={statusColors[task.status]}
                      >
                        {statusTaskLabels[task.status]}
                      </Badge>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </CardContent>
        </Card>
      </div>
      <Card className="border-border/50 bg-card/50">
        <CardHeader>
          <CardTitle className="text-lg font-semibold text-foreground">
            Resumen de Progreso
          </CardTitle>
          <CardDescription>Estado general de tus tareas</CardDescription>
        </CardHeader>
        <CardContent>
          {tasksLoading ? (
            <Skeleton className="h-8 w-full" />
          ) : tasks.length === 0 ? (
            <p className="text-sm text-muted-foreground">
              No hay tareas para mostrar
            </p>
          ) : (
            <div className="space-y-4">
              <div className="flex h-4 overflow-hidden rounded-full bg-secondary">
                {completedTasks.length > 0 && (
                  <div
                    className="bg-green-500 transition-all"
                    style={{
                      width: `${(completedTasks.length / tasks.length) * 100}%`,
                    }}
                  />
                )}
                {inProgressTasks.length > 0 && (
                  <div
                    className="bg-blue-500 transition-all"
                    style={{
                      width: `${(inProgressTasks.length / tasks.length) * 100}%`,
                    }}
                  />
                )}
                {pendingTasks.length > 0 && (
                  <div
                    className="bg-yellow-500 transition-all"
                    style={{
                      width: `${(pendingTasks.length / tasks.length) * 100}%`,
                    }}
                  />
                )}
              </div>
              <div className="flex flex-wrap gap-4 text-sm">
                <div className="flex items-center gap-2">
                  <div className="h-3 w-3 rounded-full bg-green-500" />
                  <span className="text-muted-foreground">
                    Completadas: {completedTasks.length}
                  </span>
                </div>
                <div className="flex items-center gap-2">
                  <div className="h-3 w-3 rounded-full bg-blue-500" />
                  <span className="text-muted-foreground">
                    En Progreso: {inProgressTasks.length}
                  </span>
                </div>
                <div className="flex items-center gap-2">
                  <div className="h-3 w-3 rounded-full bg-yellow-500" />
                  <span className="text-muted-foreground">
                    Pendientes: {pendingTasks.length}
                  </span>
                </div>
              </div>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
