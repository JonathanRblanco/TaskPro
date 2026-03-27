'use client';

import { useState } from 'react';
import { Task, TaskStatus } from '@/types';
import { ProjectMember } from '@/types';
import { tasksService } from '@/lib/api';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import {
  CheckCircle2,
  Circle,
  Clock,
  Loader2,
  MoreVertical,
  UserCheck,
} from 'lucide-react';
import { toast } from 'sonner';
import { useAuth } from '@/contexts/auth-context';

interface TaskCardProps {
  task: Task;
  onUpdate?: () => void;
  showProject?: boolean;
  members?: ProjectMember[];
}

const statusColors: Record<TaskStatus, string> = {
  Pending: 'border-yellow-500/30 bg-yellow-500/20 text-yellow-400',
  InProgress: 'border-blue-500/30 bg-blue-500/20 text-blue-400',
  Done: 'border-green-500/30 bg-green-500/20 text-green-400',
  Cancelled: 'border-red-500/30 bg-red-500/20 text-red-400',
};

const statusOptions: TaskStatus[] = ['Pending', 'InProgress', 'Done', 'Cancelled'];
const statusLabels: Record<string, string> = {
  Done: 'Completado',
  InProgress: 'En progreso',
  Pending: 'Pendiente',
  Cancelled: 'Cancelado',
};

export function TaskCard({ task, onUpdate, showProject = true, members }: TaskCardProps) {
  const { user } = useAuth();
  const canManage = user?.rol === 'Admin' || user?.rol === 'Manager';

  const [isUpdating, setIsUpdating] = useState(false);
  const [isAssigning, setIsAssigning] = useState(false);

  const handleStatusChange = async (newStatus: TaskStatus) => {
    if (newStatus === task.status) return;
    setIsUpdating(true);
    try {
      await tasksService.updateStatus(task.id, newStatus);
      toast.success(`Tarea actualizada a "${statusLabels[newStatus]}"`);
      onUpdate?.();
    } catch {
      toast.error('Error al actualizar la tarea');
    } finally {
      setIsUpdating(false);
    }
  };

  const handleAssign = async (userId: string, fullName: string) => {
    if (userId === task.assignedToMemberId) return;
    setIsAssigning(true);
    try {
      await tasksService.assign(task.id, userId);
      toast.success(`Tarea asignada a ${fullName}`);
      onUpdate?.();
    } catch {
      toast.error('Error al reasignar la tarea');
    } finally {
      setIsAssigning(false);
    }
  };

  const isBusy = isUpdating || isAssigning;

  return (
    <Card className="border-border/50 bg-secondary/30 transition-all hover:bg-secondary/50">
      <CardContent className="p-4">
        <div className="flex items-start justify-between gap-4">
          <div className="min-w-0 flex-1">
            <h4 className="truncate font-medium text-foreground">{task.title}</h4>
            <p className="mt-1 line-clamp-2 text-sm text-muted-foreground">{task.description}</p>

            <div className="mt-3 flex flex-wrap items-center gap-2">
              <Badge variant="outline" className={statusColors[task.status]}>
                {task.status === 'Done' && <CheckCircle2 className="mr-1 h-3 w-3" />}
                {statusLabels[task.status]}
              </Badge>

              {showProject && (
                <Badge variant="outline" className="border-primary/30 bg-primary/10 text-primary">
                  {task.projectName}
                </Badge>
              )}

              <Badge variant="outline" className="border-blue-500/30 bg-blue-500/10 text-blue-400">
                Asignado a {task.assignedToName}
              </Badge>
            </div>
          </div>

          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant="ghost" size="icon" className="shrink-0" disabled={isBusy}>
                {isBusy ? (
                  <Loader2 className="h-4 w-4 animate-spin" />
                ) : (
                  <MoreVertical className="h-4 w-4" />
                )}
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              <DropdownMenuLabel>Cambiar estado</DropdownMenuLabel>
              <DropdownMenuSeparator />
              {statusOptions.map((status) => (
                <DropdownMenuItem
                  key={status}
                  onClick={() => handleStatusChange(status)}
                  className={task.status === status ? 'bg-accent' : ''}
                >
                  {status === 'Done' && <CheckCircle2 className="mr-2 h-4 w-4 text-green-400" />}
                  {status === 'InProgress' && <Clock className="mr-2 h-4 w-4 text-blue-400" />}
                  {status === 'Pending' && <Circle className="mr-2 h-4 w-4 text-yellow-400" />}
                  {status === 'Cancelled' && <Circle className="mr-2 h-4 w-4 text-red-400" />}
                  {statusLabels[status] ?? status}
                </DropdownMenuItem>
              ))}

              {canManage && members && members.length > 0 && (
                <>
                  <DropdownMenuSeparator />
                  <DropdownMenuLabel>Reasignar a</DropdownMenuLabel>
                  {members.map((m) => (
                    <DropdownMenuItem
                      key={m.userId}
                      onClick={() => handleAssign(m.userId, m.fullName)}
                      className={task.assignedToMemberId === m.id && m.email === user.email ? 'bg-accent' : ''}
                    >
                      <UserCheck className="mr-2 h-4 w-4 text-muted-foreground" />
                      {m.fullName}
                      {task.assignedToMemberId === m.id && m.email === user.email && (
                        <span className="ml-auto text-xs text-muted-foreground">Actual</span>
                      )}
                    </DropdownMenuItem>
                  ))}
                </>
              )}
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </CardContent>
    </Card>
  );
}