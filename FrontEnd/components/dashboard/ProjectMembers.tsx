'use client';

import { useState } from 'react';
import useSWR from 'swr';
import { ProjectMember, AssignableUser } from '@/types';
import { projectsService } from '@/lib/api';
import { usersService } from '@/lib/api/users';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
  DialogFooter,
  DialogDescription,
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
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Label } from '@/components/ui/label';
import { Users, UserPlus, Trash2, Loader2 } from 'lucide-react';
import { toast } from 'sonner';
import { formatDistanceToNow } from 'date-fns';
import { es } from 'date-fns/locale';
import { useAuth } from '@/contexts/auth-context';

interface ProjectMembersProps {
  projectId: string;
}

const roleColors: Record<string, string> = {
  Manager: 'border-purple-500/30 bg-purple-500/10 text-purple-400',
  Contributor: 'border-blue-500/30 bg-blue-500/10 text-blue-400',
};

const roleLabels: Record<string, string> = {
  ProjectManager: 'Manager',
  Contributor: 'Colaborador',
  Owner: 'Creador',
};

export function ProjectMembers({ projectId }: ProjectMembersProps) {
  const { user } = useAuth();
  const canManage = user?.rol === 'Admin' || user?.rol === 'Manager';

  const [addOpen, setAddOpen] = useState(false);
  const [selectedUserId, setSelectedUserId] = useState('');
  const [selectedRole, setSelectedRole] = useState('Contributor');
  const [isAdding, setIsAdding] = useState(false);
  const [removingId, setRemovingId] = useState<string | null>(null);

  const { data: members = [], isLoading ,mutate: mutateMembers, } = useSWR<ProjectMember[]>(
    `project-members-${projectId}`,
    () => projectsService.getMembers(projectId)
  );

  const { data: assignableUsers = [] } = useSWR<AssignableUser[]>(
    canManage && addOpen ? 'assignable-users' : null,
    usersService.getAssignable
  );

  const availableUsers = assignableUsers.filter(
    (u) =>
      u.email !== user?.email &&
      !members.some((m) => m.email === u.email)
  );

  const handleAdd = async () => {
    if (!selectedUserId) {
      toast.error('Selecciona un usuario');
      return;
    }
    setIsAdding(true);
    try {
      await projectsService.addMember(projectId, selectedUserId, selectedRole);
      toast.success('Miembro añadido correctamente');
      setAddOpen(false);
      setSelectedUserId('');
      setSelectedRole('Contributor');
      mutateMembers();
    } catch {
      toast.error('Error al añadir el miembro');
    } finally {
      setIsAdding(false);
    }
  };

  const handleRemove = async (userId: string, name: string) => {
    setRemovingId(userId);
    try {
      await projectsService.removeMember(projectId, userId);
      toast.success(`${name} eliminado del proyecto`);
      mutateMembers();
    } catch {
      toast.error('Error al eliminar el miembro');
    } finally {
      setRemovingId(null);
    }
  };

  return (
    <Card className="border-border/50 bg-card/50">
      <CardHeader>
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2">
            <Users className="h-5 w-5 text-primary" />
            <CardTitle className="text-lg">
              Miembros
              {!isLoading && (
                <span className="ml-2 text-sm font-normal text-muted-foreground">
                  ({members.length})
                </span>
              )}
            </CardTitle>
          </div>

          {canManage && (
            <Dialog open={addOpen} onOpenChange={setAddOpen}>
              <DialogTrigger asChild>
                <Button size="sm" className="gap-2">
                  <UserPlus className="h-4 w-4" />
                  Añadir
                </Button>
              </DialogTrigger>
              <DialogContent className="sm:max-w-md">
                <DialogHeader>
                  <DialogTitle>Añadir miembro</DialogTitle>
                  <DialogDescription>
                    Selecciona un usuario y su rol en el proyecto.
                  </DialogDescription>
                </DialogHeader>
                <div className="flex flex-col gap-4 py-2">
                  <div className="flex flex-col gap-1.5">
                    <Label>Usuario</Label>
                    <Select value={selectedUserId} onValueChange={setSelectedUserId}>
                      <SelectTrigger>
                        <SelectValue placeholder="Selecciona un usuario..." />
                      </SelectTrigger>
                      <SelectContent>
                        {availableUsers.length === 0 ? (
                          <div className="px-3 py-2 text-sm text-muted-foreground">
                            No hay usuarios disponibles
                          </div>
                        ) : (
                          availableUsers.map((u) => (
                            <SelectItem key={u.id} value={u.id}>
                              <div className="flex flex-col">
                                <span>{u.fullName}</span>
                                <span className="text-xs text-muted-foreground">{u.email}</span>
                              </div>
                            </SelectItem>
                          ))
                        )}
                      </SelectContent>
                    </Select>
                  </div>
                  <div className="flex flex-col gap-1.5">
                    <Label>Rol</Label>
                    <Select value={selectedRole} onValueChange={setSelectedRole}>
                      <SelectTrigger>
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="Manager">Manager</SelectItem>
                        <SelectItem value="Contributor">Colaborador</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                </div>
                <DialogFooter>
                  <Button variant="outline" onClick={() => setAddOpen(false)} disabled={isAdding}>
                    Cancelar
                  </Button>
                  <Button onClick={handleAdd} disabled={isAdding} className="gap-2">
                    {isAdding && <Loader2 className="h-4 w-4 animate-spin" />}
                    Añadir miembro
                  </Button>
                </DialogFooter>
              </DialogContent>
            </Dialog>
          )}
        </div>
      </CardHeader>

      <CardContent className="p-0">
        {isLoading ? (
          <div className="flex items-center justify-center py-8">
            <Loader2 className="h-5 w-5 animate-spin text-muted-foreground" />
          </div>
        ) : members.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-10 text-center">
            <Users className="h-10 w-10 text-muted-foreground/40" />
            <p className="mt-2 text-sm text-muted-foreground">No hay miembros en este proyecto</p>
          </div>
        ) : (
          <ul className="divide-y divide-border/50">
            {members.map((member) => (
              <li key={member.id} className="flex items-center justify-between px-6 py-3">
                <div className="flex items-center gap-3">
                  <Avatar className="h-8 w-8">
                    <AvatarFallback className="text-xs">
                      {member.fullName.slice(0, 2).toUpperCase()}
                    </AvatarFallback>
                  </Avatar>
                  <div>
                    <p className="text-sm font-medium text-foreground">{member.fullName}</p>
                    <p className="text-xs text-muted-foreground">{member.email}</p>
                  </div>
                </div>

                <div className="flex items-center gap-3">
                  <div className="hidden sm:flex flex-col items-end gap-1">
                    <Badge
                      variant="outline"
                      className={roleColors[member.role] ?? 'border-border text-muted-foreground'}
                    >
                      {roleLabels[member.role] ?? member.role}
                    </Badge>
                    <span className="text-[11px] text-muted-foreground">
                      Desde{' '}
                      {formatDistanceToNow(new Date(member.joinedAt), {
                        addSuffix: true,
                        locale: es,
                      })}
                    </span>
                  </div>

                  {canManage && !(member.email === user?.email && user?.rol !== 'Contributor') && (
                    <AlertDialog>
                      <AlertDialogTrigger asChild>
                        <Button
                          variant="ghost"
                          size="icon"
                          className="h-8 w-8 text-muted-foreground hover:text-red-400 hover:bg-red-400/10"
                          disabled={removingId === member.userId}
                        >
                          {removingId === member.userId ? (
                            <Loader2 className="h-4 w-4 animate-spin" />
                          ) : (
                            <Trash2 className="h-4 w-4" />
                          )}
                        </Button>
                      </AlertDialogTrigger>
                      <AlertDialogContent>
                        <AlertDialogHeader>
                          <AlertDialogTitle>¿Eliminar miembro?</AlertDialogTitle>
                          <AlertDialogDescription>
                            Se eliminará a{' '}
                            <span className="font-medium text-foreground">{member.fullName}</span>{' '}
                            del proyecto. Podrás volver a añadirlo más tarde.
                          </AlertDialogDescription>
                        </AlertDialogHeader>
                        <AlertDialogFooter>
                          <AlertDialogCancel>Cancelar</AlertDialogCancel>
                          <AlertDialogAction
                            onClick={() => handleRemove(member.userId, member.fullName)}
                            className="bg-red-500 hover:bg-red-600"
                          >
                            Eliminar
                          </AlertDialogAction>
                        </AlertDialogFooter>
                      </AlertDialogContent>
                    </AlertDialog>
                  )}
                </div>
              </li>
            ))}
          </ul>
        )}
      </CardContent>
    </Card>
  );
}