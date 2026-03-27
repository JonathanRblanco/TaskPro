'use client';

import { useState, useMemo } from 'react';
import { useMyTasks } from '@/hooks/use-tasks';
import { Card, CardContent} from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import { Input } from '@/components/ui/input';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { CheckSquare, Search, Calendar, AlertTriangle, Check } from 'lucide-react';
import { TaskStatus } from '@/types';
import { TaskCardWithComments } from '@/components/dashboard/TaskCardWithComments';

const statusTabs: { value: string; label: string; status?: TaskStatus }[] = [
  { value: 'all', label: 'Todas' },
  { value: 'Pendiente', label: 'Pendientes', status: 'Pending' },
  { value: 'EnProgreso', label: 'En Progreso', status: 'InProgress' },
  { value: 'Completada', label: 'Completadas', status: 'Done' },
  { value: 'Cancelada', label: 'Canceladas', status: 'Cancelled' },
];

export default function TasksPage() {
  const { tasks, isLoading, mutate } = useMyTasks();
  const [search, setSearch] = useState('');
  const [priorityFilter, setPriorityFilter] = useState<string>('all');
  const [activeTab, setActiveTab] = useState('all');

  const filteredTasks = useMemo(() => {
    return tasks.filter((task) => {
      const matchesSearch =
        task.title.toLowerCase().includes(search.toLowerCase()) ||
        task.description.toLowerCase().includes(search.toLowerCase());
      const matchesStatus =
        activeTab === 'all' || task.status === activeTab;
      return matchesSearch && matchesStatus;
    });
  }, [tasks, search, priorityFilter, activeTab]);

  const taskCounts = useMemo(() => {
    return {
      all: tasks.length,
      Pendiente: tasks.filter((t) => t.status === 'Pending').length,
      EnProgreso: tasks.filter((t) => t.status === 'InProgress').length,
      Completada: tasks.filter((t) => t.status === 'Done').length,
      Cancelada: tasks.filter((t) => t.status === 'Cancelled').length,
    };
  }, [tasks]);

  const overdueTasks = useMemo(() => {
    return tasks.filter(
      (t) =>
        t.status !== 'Done'
    );
  }, [tasks]);

  return (
    <div className="space-y-6">
      <div className="pt-12 md:pt-0">
        <h1 className="text-3xl font-bold text-foreground">Mis Tareas</h1>
        <p className="mt-1 text-muted-foreground">
          Gestiona y da seguimiento a todas tus tareas asignadas
        </p>
      </div>
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <Card className="border-border/50 bg-card/50">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="rounded-lg bg-primary/10 p-2">
                <CheckSquare className="h-5 w-5 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Total</p>
                {isLoading ? (
                  <Skeleton className="h-6 w-8" />
                ) : (
                  <p className="text-xl font-bold text-foreground">{taskCounts.all}</p>
                )}
              </div>
            </div>
          </CardContent>
        </Card>
        <Card className="border-border/50 bg-card/50">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="rounded-lg bg-primary/10 p-2">
                <Check className="h-5 w-5 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Total</p>
                {isLoading ? (
                  <Skeleton className="h-6 w-8" />
                ) : (
                  <p className="text-xl font-bold text-foreground">{taskCounts.Completada}</p>
                )}
              </div>
            </div>
          </CardContent>
        </Card>
        <Card className="border-border/50 bg-card/50">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="rounded-lg bg-yellow-500/10 p-2">
                <Calendar className="h-5 w-5 text-yellow-400" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Pendientes</p>
                {isLoading ? (
                  <Skeleton className="h-6 w-8" />
                ) : (
                  <p className="text-xl font-bold text-foreground">
                    {taskCounts.Pendiente}
                  </p>
                )}
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-border/50 bg-card/50">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="rounded-lg bg-blue-500/10 p-2">
                <CheckSquare className="h-5 w-5 text-blue-400" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">En Progreso</p>
                {isLoading ? (
                  <Skeleton className="h-6 w-8" />
                ) : (
                  <p className="text-xl font-bold text-foreground">
                    {taskCounts.EnProgreso}
                  </p>
                )}
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-border/50 bg-card/50">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="rounded-lg bg-red-500/10 p-2">
                <AlertTriangle className="h-5 w-5 text-red-400" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Vencidas</p>
                {isLoading ? (
                  <Skeleton className="h-6 w-8" />
                ) : (
                  <p className="text-xl font-bold text-foreground">
                    {overdueTasks.length}
                  </p>
                )}
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
      <Card className="border-border/50 bg-card/50">
        <CardContent className="p-4">
          <div className="flex flex-col gap-4 sm:flex-row sm:items-center">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <Input
                placeholder="Buscar tareas..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="pl-10 bg-input border-border"
              />
            </div>
            <Select value={priorityFilter} onValueChange={setPriorityFilter}>
              <SelectTrigger className="w-full sm:w-48 bg-input border-border">
                <SelectValue placeholder="Filtrar por prioridad" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Todas las prioridades</SelectItem>
                <SelectItem value="Urgente">Urgente</SelectItem>
                <SelectItem value="Alta">Alta</SelectItem>
                <SelectItem value="Media">Media</SelectItem>
                <SelectItem value="Baja">Baja</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </CardContent>
      </Card>

      <Tabs value={activeTab} onValueChange={setActiveTab}>
        <TabsList className="grid w-full grid-cols-4 bg-secondary/50">
          {statusTabs.map((tab) => (
            <TabsTrigger
              key={tab.value}
              value={tab.value}
              className="data-[state=active]:bg-primary data-[state=active]:text-primary-foreground"
            >
              {tab.label}
              {!isLoading && (
                <Badge variant="secondary" className="ml-2 text-xs">
                  {tab.value === 'all'
                    ? taskCounts.all
                    : taskCounts[tab.value as keyof typeof taskCounts] || 0}
                </Badge>
              )}
            </TabsTrigger>
          ))}
        </TabsList>

        <TabsContent value={activeTab} className="mt-4">
          {isLoading ? (
            <div className="space-y-4">
              {[1, 2, 3, 4].map((i) => (
                <Skeleton key={i} className="h-28 w-full" />
              ))}
            </div>
          ) : filteredTasks.length === 0 ? (
            <Card className="border-border/50 bg-card/50">
              <CardContent className="flex flex-col items-center justify-center py-16">
                <CheckSquare className="h-16 w-16 text-muted-foreground/50" />
                <h3 className="mt-4 text-lg font-semibold text-foreground">
                  No se encontraron tareas
                </h3>
                <p className="mt-1 text-sm text-muted-foreground">
                  {search || priorityFilter !== 'all'
                    ? 'Intenta ajustar los filtros de búsqueda'
                    : 'No tienes tareas asignadas en esta categoria'}
                </p>
              </CardContent>
            </Card>
          ) : (
            <div className="space-y-3">
              {filteredTasks.map((task) => (
                <TaskCardWithComments
                  key={task.id}
                  task={task}
                  onUpdate={() => mutate()}
                  showProject
                />
              ))}
            </div>
          )}
        </TabsContent>
      </Tabs>
    </div>
  );
}
