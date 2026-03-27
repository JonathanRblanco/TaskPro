'use client';

import { useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { useProject } from '@/hooks/use-projects';
import { useProjectTasks } from '@/hooks/use-tasks';
import { useProjectMembers } from '@/hooks/use-members';
import { tasksService } from '@/lib/api/tasks';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { Skeleton } from '@/components/ui/skeleton';
import { Progress } from '@/components/ui/progress';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from '@/components/ui/alert-dialog';
import {
  ArrowLeft,
  Calendar,
  FolderKanban,
  CheckSquare,
  User,
  Plus,
  Trash2,
  Loader2,
} from 'lucide-react';
import { TaskCardWithComments } from '@/components/dashboard/TaskCardWithComments';
import { ProjectMembers } from '@/components/dashboard/ProjectMembers';
import { ProjectStatus, TaskStatus } from '@/types';
import { toast } from 'sonner';
import { useAuth } from '@/contexts/auth-context';

const statusColors: Record<ProjectStatus, string> = {
  Active: 'border-green-500/30 bg-green-500/20 text-green-400',
  Completed: 'border-blue-500/30 bg-blue-500/20 text-blue-400',
  Archived: 'border-yellow-500/30 bg-yellow-500/20 text-yellow-400',
  Cancelled: 'border-red-500/30 bg-red-500/20 text-red-400',
};

const statusLabelsProject: Record<ProjectStatus, string> = {
  Active: 'Activo',
  Completed: 'Completado',
  Archived: 'Pausado',
  Cancelled: 'Cancelado',
};

const taskStatusOrder: TaskStatus[] = ['Pending', 'InProgress', 'Done', 'Cancelled'];
const statusLabels: Record<string, string> = {
  Done: 'Completado',
  InProgress: 'En progreso',
  Pending: 'Pendiente',
  Cancelled: 'Cancelado',
};

const EMPTY_TASK = { title: '', description: '', assignedToId: '', dueDate: '' };

export default function ProjectDetailPage() {
  const params = useParams();
  const router = useRouter();
  const projectId = params.id as string;
  const { user } = useAuth();

  const canManage = user?.rol === 'Admin' || user?.rol === 'Manager';

  const { project, isLoading: projectLoading } = useProject(projectId);
  const { tasks, isLoading: tasksLoading, mutate: mutateTasks } = useProjectTasks(projectId);
  const { members } = useProjectMembers(projectId); // para el select de asignación

  const [createOpen, setCreateOpen] = useState(false);
  const [taskForm, setTaskForm] = useState(EMPTY_TASK);
  const [isCreating, setIsCreating] = useState(false);
  const [deletingId, setDeletingId] = useState<string | null>(null);

  const formatDate = (dateString: string | null) => {
    if (!dateString) return 'Sin definir';
    return new Date(dateString).toLocaleDateString('es-ES', {
      day: 'numeric',
      month: 'long',
      year: 'numeric',
    });
  };

  const completedTasks = tasks.filter((t) => t.status === 'Done');
  const progress = tasks.length > 0 ? (completedTasks.length / tasks.length) * 100 : 0;

  const groupedTasks = taskStatusOrder.reduce((acc, status) => {
    acc[status] = tasks.filter((t) => t.status === status);
    return acc;
  }, {} as Record<TaskStatus, typeof tasks>);

  const handleCreateTask = async () => {
    if (!taskForm.title.trim()) {
      toast.error('El título es obligatorio');
      return;
    }
    setIsCreating(true);
    try {
      await tasksService.create({
        title: taskForm.title,
        description: taskForm.description,
        projectId,
        dueDate: taskForm.dueDate || undefined,
      });
      toast.success('Tarea creada correctamente');
      setCreateOpen(false);
      setTaskForm(EMPTY_TASK);
      mutateTasks();
    } catch {
      toast.error('Error al crear la tarea');
    } finally {
      setIsCreating(false);
    }
  };

  const handleDeleteTask = async (id: string) => {
    setDeletingId(id);
    try {
      await tasksService.delete(id);
      toast.success('Tarea eliminada');
      mutateTasks();
    } catch {
      toast.error('Error al eliminar la tarea');
    } finally {
      setDeletingId(null);
    }
  };

  if (projectLoading) {
    return (
      <div className="space-y-6">
        <div className="flex items-center gap-4 pt-12 md:pt-0">
          <Skeleton className="h-10 w-10" />
          <Skeleton className="h-8 w-64" />
        </div>
        <div className="grid gap-4 lg:grid-cols-3">
          <Skeleton className="h-48 lg:col-span-2" />
          <Skeleton className="h-48" />
        </div>
        <Skeleton className="h-64 w-full" />
      </div>
    );
  }

  if (!project) {
    return (
      <div className="flex flex-col items-center justify-center py-16">
        <FolderKanban className="h-16 w-16 text-muted-foreground/50" />
        <h2 className="mt-4 text-xl font-semibold text-foreground">Proyecto no encontrado</h2>
        <p className="mt-1 text-muted-foreground">
          El proyecto que buscas no existe o no tienes acceso
        </p>
        <Button variant="outline" className="mt-4" onClick={() => router.push('/dashboard/projects')}>
          <ArrowLeft className="mr-2 h-4 w-4" />
          Volver a proyectos
        </Button>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 pt-12 md:pt-0 sm:flex-row sm:items-center sm:justify-between">
        <div className="flex items-center gap-4">
          <Button variant="ghost" size="icon" onClick={() => router.push('/dashboard/projects')} className="shrink-0">
            <ArrowLeft className="h-5 w-5" />
          </Button>
          <div>
            <div className="flex items-center gap-3">
              <h1 className="text-2xl font-bold text-foreground md:text-3xl">{project.name}</h1>
              <Badge variant="outline" className={statusColors[project.status]}>
                {statusLabelsProject[project.status]}
              </Badge>
            </div>
            <p className="mt-1 text-muted-foreground">{project.description}</p>
          </div>
        </div>
      </div>

      <div className="grid gap-4 lg:grid-cols-3">
        <Card className="border-border/50 bg-card/50 lg:col-span-2">
          <CardHeader>
            <CardTitle className="text-lg font-semibold text-foreground">Información del Proyecto</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid gap-6 sm:grid-cols-2">
              <div className="flex items-center gap-3">
                <div className="rounded-lg bg-primary/10 p-2">
                  <Calendar className="h-5 w-5 text-primary" />
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Fecha de Inicio</p>
                  <p className="font-medium text-foreground">{formatDate(project.createdAt)}</p>
                </div>
              </div>
              <div className="flex items-center gap-3">
                <div className="rounded-lg bg-yellow-500/10 p-2">
                  <CheckSquare className="h-5 w-5 text-yellow-400" />
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Total de Tareas</p>
                  <p className="font-medium text-foreground">{tasks.length}</p>
                </div>
              </div>
              <div className="flex items-center gap-3">
                <div className="rounded-lg bg-green-500/10 p-2">
                  <User className="h-5 w-5 text-green-400" />
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Creador</p>
                  <p className="font-medium text-foreground">{project.ownerName}</p>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-border/50 bg-card/50">
          <CardHeader>
            <CardTitle className="text-lg font-semibold text-foreground">Progreso</CardTitle>
            <CardDescription>{completedTasks.length} de {tasks.length} tareas completadas</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <Progress value={progress} className="h-3" />
            <p className="text-center text-2xl font-bold text-primary">{progress.toFixed(0)}%</p>
            <div className="grid grid-cols-2 gap-2 text-center text-sm">
              <div className="rounded-lg bg-secondary/50 p-2">
                <p className="font-semibold text-yellow-400">{groupedTasks.Pending.length}</p>
                <p className="text-muted-foreground">Pendientes</p>
              </div>
              <div className="rounded-lg bg-secondary/50 p-2">
                <p className="font-semibold text-blue-400">{groupedTasks.InProgress.length}</p>
                <p className="text-muted-foreground">En Progreso</p>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      <div className="grid gap-4 lg:grid-cols-3">
        <div className="lg:col-span-1">
          <ProjectMembers projectId={projectId} />
        </div>

        <Card className="border-border/50 bg-card/50 lg:col-span-2">
          <CardHeader>
            <div className="flex items-center justify-between">
              <div>
                <CardTitle className="text-lg font-semibold text-foreground">Tareas del Proyecto</CardTitle>
                <CardDescription>Gestiona las tareas asignadas a este proyecto</CardDescription>
              </div>
              {canManage && (
                <Button size="sm" className="gap-2" onClick={() => setCreateOpen(true)}>
                  <Plus className="h-4 w-4" />
                  Nueva tarea
                </Button>
              )}
            </div>
          </CardHeader>
          <CardContent>
            {tasksLoading ? (
              <div className="space-y-4">
                {[1, 2, 3].map((i) => <Skeleton key={i} className="h-24 w-full" />)}
              </div>
            ) : tasks.length === 0 ? (
              <div className="flex flex-col items-center justify-center py-12 text-center">
                <CheckSquare className="h-12 w-12 text-muted-foreground/50" />
                <h3 className="mt-4 text-lg font-semibold text-foreground">Sin tareas</h3>
                <p className="mt-1 text-sm text-muted-foreground">
                  Este proyecto aún no tiene tareas asignadas
                </p>
              </div>
            ) : (
              <div className="space-y-6">
                {taskStatusOrder.map((status) => {
                  const statusTasks = groupedTasks[status];
                  if (statusTasks.length === 0) return null;
                  return (
                    <div key={status}>
                      <div className="mb-3 flex items-center gap-2">
                        <h3 className="font-semibold text-foreground">{statusLabels[status]}</h3>
                        <Badge variant="secondary" className="text-xs">{statusTasks.length}</Badge>
                      </div>
                      <div className="space-y-3">
                        {statusTasks.map((task) => (
                          <div key={task.id} className="group/task relative">
                            <TaskCardWithComments
                              task={task}
                              members={members}
                              onUpdate={() => mutateTasks()}
                            />
                            {canManage && (
                              <AlertDialog>
                                <AlertDialogTrigger asChild>
                                  <Button
                                    variant="ghost"
                                    size="icon"
                                    className="absolute right-10 top-3 h-7 w-7 opacity-0 group-hover/task:opacity-100 transition-opacity text-muted-foreground hover:text-red-400 hover:bg-red-400/10"
                                    disabled={deletingId === task.id}
                                  >
                                    {deletingId === task.id ? (
                                      <Loader2 className="h-3.5 w-3.5 animate-spin" />
                                    ) : (
                                      <Trash2 className="h-3.5 w-3.5" />
                                    )}
                                  </Button>
                                </AlertDialogTrigger>
                                <AlertDialogContent>
                                  <AlertDialogHeader>
                                    <AlertDialogTitle>¿Eliminar tarea?</AlertDialogTitle>
                                    <AlertDialogDescription>
                                      Esta acción no se puede deshacer. Se eliminará la tarea{' '}
                                      <span className="font-medium text-foreground">"{task.title}"</span>.
                                    </AlertDialogDescription>
                                  </AlertDialogHeader>
                                  <AlertDialogFooter>
                                    <AlertDialogCancel>Cancelar</AlertDialogCancel>
                                    <AlertDialogAction
                                      onClick={() => handleDeleteTask(task.id)}
                                      className="bg-red-500 hover:bg-red-600"
                                    >
                                      Eliminar
                                    </AlertDialogAction>
                                  </AlertDialogFooter>
                                </AlertDialogContent>
                              </AlertDialog>
                            )}
                          </div>
                        ))}
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </CardContent>
        </Card>
      </div>

      <Dialog open={createOpen} onOpenChange={setCreateOpen}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Nueva tarea</DialogTitle>
            <DialogDescription>Completa los datos de la nueva tarea.</DialogDescription>
          </DialogHeader>
          <div className="flex flex-col gap-4 py-2">
            <div className="flex flex-col gap-1.5">
              <Label>Título *</Label>
              <Input
                placeholder="Título de la tarea"
                value={taskForm.title}
                onChange={(e) => setTaskForm((p) => ({ ...p, title: e.target.value }))}
              />
            </div>
            <div className="flex flex-col gap-1.5">
              <Label>Descripción</Label>
              <Textarea
                placeholder="Describe la tarea..."
                rows={3}
                value={taskForm.description}
                onChange={(e) => setTaskForm((p) => ({ ...p, description: e.target.value }))}
              />
            </div>
            <div className="flex flex-col gap-1.5">
              <Label>Fecha límite</Label>
              <Input
                type="date"
                value={taskForm.dueDate}
                onChange={(e) => setTaskForm((p) => ({ ...p, dueDate: e.target.value }))}
              />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setCreateOpen(false)} disabled={isCreating}>
              Cancelar
            </Button>
            <Button onClick={handleCreateTask} disabled={isCreating} className="gap-2">
              {isCreating && <Loader2 className="h-4 w-4 animate-spin" />}
              Crear tarea
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}