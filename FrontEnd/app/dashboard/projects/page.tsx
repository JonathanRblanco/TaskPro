'use client';

import { useMyProjects } from '@/hooks/use-projects';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { FolderKanban, Calendar, Search, ArrowRight, Plus, Loader2, Trash2 } from 'lucide-react';
import Link from 'next/link';
import { useState, useMemo } from 'react';
import { ProjectStatus } from '@/types';
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle, AlertDialogTrigger } from '@/components/ui/alert-dialog';
import { useAuth } from '@/contexts/auth-context';
import { projectsService } from '@/lib/api';
import { toast } from 'sonner';
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle, DialogTrigger } from '@/components/ui/dialog';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';

const statusColors: Record<ProjectStatus, string> = {
  Active: 'border-green-500/30 bg-green-500/20 text-green-400',
  Completed: 'border-blue-500/30 bg-blue-500/20 text-blue-400',
  Archived: 'border-yellow-500/30 bg-yellow-500/20 text-yellow-400',
  Cancelled: 'border-red-500/30 bg-red-500/20 text-red-400',
};
const statusLabels: Record<ProjectStatus, string> = {
  Active: 'Activo',
  Completed: 'Completado',
  Archived: 'Pausado',
  Cancelled: 'Cancelado',
};
export default function ProjectsPage() {
  const { projects, isLoading, mutate } = useMyProjects();
  const { user } = useAuth();
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('all');

  const [createOpen, setCreateOpen] = useState(false);
  const [isCreating, setIsCreating] = useState(false);
  const [newProject, setNewProject] = useState({ name: '', description: '' });

  const [deletingId, setDeletingId] = useState<string | null>(null);

  const canManage = user?.rol === 'Admin' || user?.rol === 'Manager';

  const filteredProjects = useMemo(() => {
    return projects.filter((project) => {
      const matchesSearch =
        project.name.toLowerCase().includes(search.toLowerCase()) ||
        project.description.toLowerCase().includes(search.toLowerCase());
      const matchesStatus = statusFilter === 'all' || project.status === statusFilter;
      return matchesSearch && matchesStatus;
    });
  }, [projects, search, statusFilter]);

  const handleCreate = async () => {
    if (!newProject.name.trim()) {
      toast.error('El nombre es obligatorio');
      return;
    }
    setIsCreating(true);
    try {
      await projectsService.create(newProject);
      toast.success('Proyecto creado correctamente');
      setCreateOpen(false);
      setNewProject({ name: '', description: '' });
      mutate();
    } catch {
      toast.error('Error al crear el proyecto');
    } finally {
      setIsCreating(false);
    }
  };

  const handleDelete = async (id: string) => {
    setDeletingId(id);
    try {
      await projectsService.delete(id);
      toast.success('Proyecto eliminado');
      mutate();
    } catch {
      toast.error('Error al eliminar el proyecto');
    } finally {
      setDeletingId(null);
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('es-ES', {
      day: 'numeric',
      month: 'short',
      year: 'numeric',
    });
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between pt-12 md:pt-0">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Proyectos</h1>
          <p className="mt-1 text-muted-foreground">
            Gestiona y visualiza todos tus proyectos asignados
          </p>
        </div>

        {canManage && (
          <Dialog open={createOpen} onOpenChange={setCreateOpen}>
            <DialogTrigger asChild>
              <Button className="gap-2">
                <Plus className="h-4 w-4" />
                Nuevo proyecto
              </Button>
            </DialogTrigger>
            <DialogContent className="sm:max-w-md">
              <DialogHeader>
                <DialogTitle>Crear proyecto</DialogTitle>
                <DialogDescription>
                  Completa los datos para crear un nuevo proyecto.
                </DialogDescription>
              </DialogHeader>
              <div className="flex flex-col gap-4 py-2">
                <div className="flex flex-col gap-1.5">
                  <Label htmlFor="name">Nombre *</Label>
                  <Input
                    id="name"
                    placeholder="Nombre del proyecto"
                    value={newProject.name}
                    onChange={(e) => setNewProject((prev) => ({ ...prev, name: e.target.value }))}
                  />
                </div>
                <div className="flex flex-col gap-1.5">
                  <Label htmlFor="description">Descripción</Label>
                  <Textarea
                    id="description"
                    placeholder="Describe el proyecto..."
                    rows={3}
                    value={newProject.description}
                    onChange={(e) =>
                      setNewProject((prev) => ({ ...prev, description: e.target.value }))
                    }
                  />
                </div>
              </div>
              <DialogFooter>
                <Button variant="outline" onClick={() => setCreateOpen(false)} disabled={isCreating}>
                  Cancelar
                </Button>
                <Button onClick={handleCreate} disabled={isCreating} className="gap-2">
                  {isCreating && <Loader2 className="h-4 w-4 animate-spin" />}
                  Crear proyecto
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        )}
      </div>

      <Card className="border-border/50 bg-card/50">
        <CardContent className="p-4">
          <div className="flex flex-col gap-4 sm:flex-row sm:items-center">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <Input
                placeholder="Buscar proyectos..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="pl-10 bg-input border-border"
              />
            </div>
            <Select value={statusFilter} onValueChange={setStatusFilter}>
              <SelectTrigger className="w-full sm:w-48 bg-input border-border">
                <SelectValue placeholder="Filtrar por estado" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Todos los estados</SelectItem>
                <SelectItem value="Active">Activo</SelectItem>
                <SelectItem value="Completed">Completado</SelectItem>
                <SelectItem value="Archived">Pausado</SelectItem>
                <SelectItem value="Cancelled">Cancelado</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </CardContent>
      </Card>

      {isLoading ? (
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {[1, 2, 3, 4, 5, 6].map((i) => (
            <Card key={i} className="border-border/50 bg-card/50">
              <CardHeader>
                <Skeleton className="h-6 w-3/4" />
                <Skeleton className="h-4 w-full mt-2" />
              </CardHeader>
              <CardContent>
                <Skeleton className="h-4 w-1/2" />
              </CardContent>
            </Card>
          ))}
        </div>
      ) : filteredProjects.length === 0 ? (
        <Card className="border-border/50 bg-card/50">
          <CardContent className="flex flex-col items-center justify-center py-16">
            <FolderKanban className="h-16 w-16 text-muted-foreground/50" />
            <h3 className="mt-4 text-lg font-semibold text-foreground">
              No se encontraron proyectos
            </h3>
            <p className="mt-1 text-sm text-muted-foreground">
              {search || statusFilter !== 'all'
                ? 'Intenta ajustar los filtros de búsqueda'
                : 'No tienes proyectos asignados actualmente'}
            </p>
          </CardContent>
        </Card>
      ) : (
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {filteredProjects.map((project) => (
            <Card
              key={project.id}
              className="group border-border/50 bg-card/50 transition-all hover:border-primary/30 hover:shadow-lg hover:shadow-primary/5"
            >
              <CardHeader>
                <div className="flex items-start justify-between">
                  <div className="flex items-center gap-3">
                    <div className="rounded-lg bg-primary/10 p-2">
                      <FolderKanban className="h-5 w-5 text-primary" />
                    </div>
                    <div>
                      <CardTitle className="text-lg font-semibold text-foreground">
                        {project.name}
                      </CardTitle>
                    </div>
                  </div>
                  <Badge variant="outline" className={statusColors[project.status]}>
                    {statusLabels[project.status]}
                  </Badge>
                </div>
                <CardDescription className="mt-3 line-clamp-2">
                  {project.description}
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-2 text-sm text-muted-foreground">
                    <Calendar className="h-4 w-4" />
                    <span>{formatDate(project.createdAt)}</span>
                  </div>
                  <div className="flex items-center gap-1">
                    {canManage && (
                      <AlertDialog>
                        <AlertDialogTrigger asChild>
                          <Button
                            variant="ghost"
                            size="icon"
                            className="h-8 w-8 text-muted-foreground hover:text-red-400 hover:bg-red-400/10"
                            disabled={deletingId === project.id}
                          >
                            {deletingId === project.id ? (
                              <Loader2 className="h-4 w-4 animate-spin" />
                            ) : (
                              <Trash2 className="h-4 w-4" />
                            )}
                          </Button>
                        </AlertDialogTrigger>
                        <AlertDialogContent>
                          <AlertDialogHeader>
                            <AlertDialogTitle>¿Eliminar proyecto?</AlertDialogTitle>
                            <AlertDialogDescription>
                              Esta acción no se puede deshacer. Se eliminará permanentemente el
                              proyecto <span className="font-medium text-foreground">"{project.name}"</span> y
                              todos sus datos asociados.
                            </AlertDialogDescription>
                          </AlertDialogHeader>
                          <AlertDialogFooter>
                            <AlertDialogCancel>Cancelar</AlertDialogCancel>
                            <AlertDialogAction
                              onClick={() => handleDelete(project.id)}
                              className="bg-red-500 hover:bg-red-600"
                            >
                              Eliminar
                            </AlertDialogAction>
                          </AlertDialogFooter>
                        </AlertDialogContent>
                      </AlertDialog>
                    )}
                    <Button
                      asChild
                      variant="ghost"
                      size="sm"
                      className="text-primary hover:text-primary hover:bg-primary/10"
                    >
                      <Link href={`/dashboard/projects/${project.id}`}>
                        Ver detalles
                        <ArrowRight className="ml-1 h-4 w-4" />
                      </Link>
                    </Button>
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      {!isLoading && projects.length > 0 && (
        <Card className="border-border/50 bg-card/50">
          <CardContent className="p-4">
            <div className="flex flex-wrap items-center justify-center gap-6 text-sm">
              <div className="text-center">
                <p className="text-2xl font-bold text-foreground">{projects.length}</p>
                <p className="text-muted-foreground">Total Proyectos</p>
              </div>
              <div className="h-8 w-px bg-border" />
              <div className="text-center">
                <p className="text-2xl font-bold text-green-400">
                  {projects.filter((p) => p.status === 'Active').length}
                </p>
                <p className="text-muted-foreground">Activos</p>
              </div>
              <div className="h-8 w-px bg-border" />
              <div className="text-center">
                <p className="text-2xl font-bold text-blue-400">
                  {projects.filter((p) => p.status === 'Completed').length}
                </p>
                <p className="text-muted-foreground">Completados</p>
              </div>
              <div className="h-8 w-px bg-border" />
              <div className="text-center">
                <p className="text-2xl font-bold text-yellow-400">
                  {projects.filter((p) => p.status === 'Archived').length}
                </p>
                <p className="text-muted-foreground">Pausados</p>
              </div>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}