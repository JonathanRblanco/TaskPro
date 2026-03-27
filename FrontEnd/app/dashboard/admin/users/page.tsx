// app/dashboard/admin/users/page.tsx
'use client';
import useSWR from 'swr';
import { useState, useMemo, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { usersService } from '@/lib/api/users';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Skeleton } from '@/components/ui/skeleton';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
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
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import {
  Users,
  UserPlus,
  Pencil,
  Trash2,
  Search,
  Loader2,
  ShieldAlert,
} from 'lucide-react';
import { toast } from 'sonner';
import { formatDistanceToNow } from 'date-fns';
import { es } from 'date-fns/locale';
import { CreateUserDTO, UpdateUserDTO, UserDTO } from '@/types';
import { useAuth } from '@/contexts/auth-context';

const roleColors: Record<string, string> = {
  Admin: 'border-red-500/30 bg-red-500/10 text-red-400',
  Manager: 'border-purple-500/30 bg-purple-500/10 text-purple-400',
  Member: 'border-blue-500/30 bg-blue-500/10 text-blue-400',
};

const roleLabels: Record<string, string> = {
  Admin: 'Admin',
  Manager: 'Manager',
  Member: 'Miembro',
};

const EMPTY_CREATE: CreateUserDTO = {
  firstName: '',
  lastName: '',
  email: '',
  password: '',
  role: 'Member',
};

export default function AdminUsersPage() {
  const { user } = useAuth();
  const router = useRouter();

  const [search, setSearch] = useState('');
  const [roleFilter, setRoleFilter] = useState('all');

  const [createOpen, setCreateOpen] = useState(false);
  const [createForm, setCreateForm] = useState<CreateUserDTO>(EMPTY_CREATE);
  const [isCreating, setIsCreating] = useState(false);

  const [editOpen, setEditOpen] = useState(false);
  const [editingUser, setEditingUser] = useState<UserDTO | null>(null);
  const [editForm, setEditForm] = useState<UpdateUserDTO>({ firstName: '', lastName: '' });
  const [isEditing, setIsEditing] = useState(false);

  const [deletingId, setDeletingId] = useState<string | null>(null);

  const { data: users = [], isLoading, mutate } = useSWR<UserDTO[]>(
    'admin-users',
    usersService.getAll
  );

  useEffect(() => {
    if (user && user.rol !== 'Admin') {
      router.push('/dashboard');
    }
  }, [user, router]);

  if (user && user.rol !== 'Admin') return null;

  const filtered = useMemo(() => {
    return users.filter((u) => {
      const matchesSearch =
        u.fullName.toLowerCase().includes(search.toLowerCase()) ||
        u.email.toLowerCase().includes(search.toLowerCase());
      const matchesRole = roleFilter === 'all' || u.role === roleFilter;
      return matchesSearch && matchesRole;
    });
  }, [users, search, roleFilter]);

  const handleCreate = async () => {
    if (!createForm.firstName.trim() || !createForm.lastName.trim()) {
      toast.error('Nombre y apellido son obligatorios');
      return;
    }
    if (!createForm.email.trim()) {
      toast.error('El email es obligatorio');
      return;
    }
    if (!createForm.password.trim()) {
      toast.error('La contraseña es obligatoria');
      return;
    }
    setIsCreating(true);
    try {
      await usersService.create(createForm);
      toast.success('Usuario creado correctamente');
      setCreateOpen(false);
      setCreateForm(EMPTY_CREATE);
      mutate();
    } catch {
      toast.error('Error al crear el usuario');
    } finally {
      setIsCreating(false);
    }
  };

  const handleEditOpen = (u: UserDTO) => {
    setEditingUser(u);
    setEditForm({ firstName: u.firstName, lastName: u.lastName });
    setEditOpen(true);
  };

  const handleEdit = async () => {
    if (!editingUser) return;
    if (!editForm.firstName.trim() || !editForm.lastName.trim()) {
      toast.error('Nombre y apellido son obligatorios');
      return;
    }
    setIsEditing(true);
    try {
      await usersService.update(editingUser.id, editForm);
      toast.success('Usuario actualizado correctamente');
      setEditOpen(false);
      setEditingUser(null);
      mutate();
    } catch {
      toast.error('Error al actualizar el usuario');
    } finally {
      setIsEditing(false);
    }
  };

  const handleDelete = async (id: string) => {
    setDeletingId(id);
    try {
      await usersService.delete(id);
      toast.success('Usuario eliminado');
      mutate();
    } catch {
      toast.error('Error al eliminar el usuario');
    } finally {
      setDeletingId(null);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 pt-12 md:pt-0 sm:flex-row sm:items-center sm:justify-between">
        <div className="flex items-center gap-3">
          <div className="rounded-lg bg-red-500/10 p-2">
            <ShieldAlert className="h-6 w-6 text-red-400" />
          </div>
          <div>
            <h1 className="text-2xl font-bold text-foreground md:text-3xl">
              Administración de Usuarios
            </h1>
            <p className="text-sm text-muted-foreground">
              Panel exclusivo para administradores
            </p>
          </div>
        </div>
        <Button className="gap-2" onClick={() => setCreateOpen(true)}>
          <UserPlus className="h-4 w-4" />
          Nuevo usuario
        </Button>
      </div>

      {!isLoading && users.length > 0 && (
        <div className="grid grid-cols-2 gap-4 sm:grid-cols-4">
          {[
            { label: 'Total', value: users.length, color: 'text-foreground' },
            {
              label: 'Admins',
              value: users.filter((u) => u.role === 'Admin').length,
              color: 'text-red-400',
            },
            {
              label: 'Managers',
              value: users.filter((u) => u.role === 'Manager').length,
              color: 'text-purple-400',
            },
            {
              label: 'Miembros',
              value: users.filter((u) => u.role === 'Member').length,
              color: 'text-blue-400',
            },
          ].map((stat) => (
            <Card key={stat.label} className="border-border/50 bg-card/50">
              <CardContent className="p-4 text-center">
                <p className={`text-2xl font-bold ${stat.color}`}>{stat.value}</p>
                <p className="text-sm text-muted-foreground">{stat.label}</p>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      <Card className="border-border/50 bg-card/50">
        <CardContent className="p-4">
          <div className="flex flex-col gap-4 sm:flex-row sm:items-center">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <Input
                placeholder="Buscar por nombre o email..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="pl-10 bg-input border-border"
              />
            </div>
            <Select value={roleFilter} onValueChange={setRoleFilter}>
              <SelectTrigger className="w-full sm:w-48 bg-input border-border">
                <SelectValue placeholder="Filtrar por rol" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Todos los roles</SelectItem>
                <SelectItem value="Admin">Admin</SelectItem>
                <SelectItem value="Manager">Manager</SelectItem>
                <SelectItem value="Member">Miembro</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </CardContent>
      </Card>

      <Card className="border-border/50 bg-card/50">
        <CardHeader>
          <CardTitle className="flex items-center gap-2 text-lg">
            <Users className="h-5 w-5 text-primary" />
            Usuarios
            {!isLoading && (
              <span className="text-sm font-normal text-muted-foreground">
                ({filtered.length})
              </span>
            )}
          </CardTitle>
        </CardHeader>
        <CardContent className="p-0">
          {isLoading ? (
            <div className="space-y-3 p-6">
              {[1, 2, 3, 4].map((i) => (
                <Skeleton key={i} className="h-14 w-full" />
              ))}
            </div>
          ) : filtered.length === 0 ? (
            <div className="flex flex-col items-center justify-center py-16">
              <Users className="h-12 w-12 text-muted-foreground/40" />
              <p className="mt-3 text-sm text-muted-foreground">
                {search || roleFilter !== 'all'
                  ? 'No hay usuarios que coincidan con los filtros'
                  : 'No hay usuarios registrados'}
              </p>
            </div>
          ) : (
            <Table>
              <TableHeader>
                <TableRow className="border-border/50 hover:bg-transparent">
                  <TableHead>Usuario</TableHead>
                  <TableHead>Email</TableHead>
                  <TableHead>Rol</TableHead>
                  <TableHead>Miembro desde</TableHead>
                  <TableHead className="text-right">Acciones</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filtered.map((u) => (
                  <TableRow key={u.id} className="border-border/50">
                    <TableCell>
                      <div className="flex items-center gap-3">
                        <Avatar className="h-8 w-8">
                          <AvatarFallback className="text-xs">
                            {u.fullName.slice(0, 2).toUpperCase()}
                          </AvatarFallback>
                        </Avatar>
                        <span className="font-medium text-foreground">{u.fullName}</span>
                        {u.email === user?.email && (
                          <Badge variant="outline" className="text-[10px] px-1.5 py-0">
                            Tú
                          </Badge>
                        )}
                      </div>
                    </TableCell>
                    <TableCell className="text-muted-foreground">{u.email}</TableCell>
                    <TableCell>
                      <Badge
                        variant="outline"
                        className={roleColors[u.role] ?? 'border-border text-muted-foreground'}
                      >
                        {roleLabels[u.role] ?? u.role}
                      </Badge>
                    </TableCell>
                    <TableCell className="text-sm text-muted-foreground">
                      {formatDistanceToNow(new Date(u.createdAt), {
                        addSuffix: true,
                        locale: es,
                      })}
                    </TableCell>
                    <TableCell className="text-right">
                      <div className="flex items-center justify-end gap-1">
                        <Button
                          variant="ghost"
                          size="icon"
                          className="h-8 w-8 text-muted-foreground hover:text-foreground"
                          onClick={() => handleEditOpen(u)}
                        >
                          <Pencil className="h-4 w-4" />
                        </Button>
                        {u.email !== user?.email && (
                          <AlertDialog>
                            <AlertDialogTrigger asChild>
                              <Button
                                variant="ghost"
                                size="icon"
                                className="h-8 w-8 text-muted-foreground hover:text-red-400 hover:bg-red-400/10"
                                disabled={deletingId === u.id}
                              >
                                {deletingId === u.id ? (
                                  <Loader2 className="h-4 w-4 animate-spin" />
                                ) : (
                                  <Trash2 className="h-4 w-4" />
                                )}
                              </Button>
                            </AlertDialogTrigger>
                            <AlertDialogContent>
                              <AlertDialogHeader>
                                <AlertDialogTitle>¿Eliminar usuario?</AlertDialogTitle>
                                <AlertDialogDescription>
                                  Esta acción no se puede deshacer. Se eliminará permanentemente a{' '}
                                  <span className="font-medium text-foreground">{u.fullName}</span>.
                                </AlertDialogDescription>
                              </AlertDialogHeader>
                              <AlertDialogFooter>
                                <AlertDialogCancel>Cancelar</AlertDialogCancel>
                                <AlertDialogAction
                                  onClick={() => handleDelete(u.id)}
                                  className="bg-red-500 hover:bg-red-600"
                                >
                                  Eliminar
                                </AlertDialogAction>
                              </AlertDialogFooter>
                            </AlertDialogContent>
                          </AlertDialog>
                        )}
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          )}
        </CardContent>
      </Card>

      <Dialog open={createOpen} onOpenChange={setCreateOpen}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Crear usuario</DialogTitle>
            <DialogDescription>Completa los datos del nuevo usuario.</DialogDescription>
          </DialogHeader>
          <div className="flex flex-col gap-4 py-2">
            <div className="grid grid-cols-2 gap-4">
              <div className="flex flex-col gap-1.5">
                <Label>Nombre *</Label>
                <Input
                  placeholder="Juan"
                  value={createForm.firstName}
                  onChange={(e) => setCreateForm((p) => ({ ...p, firstName: e.target.value }))}
                />
              </div>
              <div className="flex flex-col gap-1.5">
                <Label>Apellido *</Label>
                <Input
                  placeholder="Pérez"
                  value={createForm.lastName}
                  onChange={(e) => setCreateForm((p) => ({ ...p, lastName: e.target.value }))}
                />
              </div>
            </div>
            <div className="flex flex-col gap-1.5">
              <Label>Email *</Label>
              <Input
                type="email"
                placeholder="juan@email.com"
                value={createForm.email}
                onChange={(e) => setCreateForm((p) => ({ ...p, email: e.target.value }))}
              />
            </div>
            <div className="flex flex-col gap-1.5">
              <Label>Contraseña *</Label>
              <Input
                type="password"
                placeholder="••••••••"
                value={createForm.password}
                onChange={(e) => setCreateForm((p) => ({ ...p, password: e.target.value }))}
              />
            </div>
            <div className="flex flex-col gap-1.5">
              <Label>Rol</Label>
              <Select
                value={createForm.role}
                onValueChange={(v) => setCreateForm((p) => ({ ...p, role: v }))}
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Admin">Admin</SelectItem>
                  <SelectItem value="Manager">Manager</SelectItem>
                  <SelectItem value="Member">Miembro</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setCreateOpen(false)} disabled={isCreating}>
              Cancelar
            </Button>
            <Button onClick={handleCreate} disabled={isCreating} className="gap-2">
              {isCreating && <Loader2 className="h-4 w-4 animate-spin" />}
              Crear usuario
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={editOpen} onOpenChange={setEditOpen}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Editar usuario</DialogTitle>
            <DialogDescription>
              Modifica el nombre de{' '}
              <span className="font-medium text-foreground">{editingUser?.fullName}</span>.
            </DialogDescription>
          </DialogHeader>
          <div className="flex flex-col gap-4 py-2">
            <div className="grid grid-cols-2 gap-4">
              <div className="flex flex-col gap-1.5">
                <Label>Nombre *</Label>
                <Input
                  value={editForm.firstName}
                  onChange={(e) => setEditForm((p) => ({ ...p, firstName: e.target.value }))}
                />
              </div>
              <div className="flex flex-col gap-1.5">
                <Label>Apellido *</Label>
                <Input
                  value={editForm.lastName}
                  onChange={(e) => setEditForm((p) => ({ ...p, lastName: e.target.value }))}
                />
              </div>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setEditOpen(false)} disabled={isEditing}>
              Cancelar
            </Button>
            <Button onClick={handleEdit} disabled={isEditing} className="gap-2">
              {isEditing && <Loader2 className="h-4 w-4 animate-spin" />}
              Guardar cambios
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}