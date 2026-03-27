'use client';

import { useState } from 'react';
import { Task, Comment, ProjectMember } from '@/types';
import { commentsService } from '@/lib/api/comments';
import { TaskCard } from './task-card';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import { MessageSquare, ChevronDown, ChevronUp, Loader2, Send } from 'lucide-react';
import { toast } from 'sonner';
import { formatDistanceToNow } from 'date-fns';
import { es } from 'date-fns/locale';

interface TaskCardWithCommentsProps {
  task: Task;
  onUpdate?: () => void;
  showProject?: boolean;
  members?: ProjectMember[];
}

export function TaskCardWithComments({ task, onUpdate, showProject, members  }: TaskCardWithCommentsProps) {
  const [open, setOpen] = useState(false);
  const [comments, setComments] = useState<Comment[]>([]);
  const [loading, setLoading] = useState(false);
  const [fetched, setFetched] = useState(false);
  const [newComment, setNewComment] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleToggle = async () => {
    if (!open && !fetched) {
      setLoading(true);
      try {
        const data = await commentsService.getByTask(task.id);
        setComments(data);
        setFetched(true);
      } catch {
        toast.error('Error al cargar los comentarios');
      } finally {
        setLoading(false);
      }
    }
    setOpen((prev) => !prev);
  };

  const handleSubmitComment = async () => {
    if (!newComment.trim()) return;
    setIsSubmitting(true);
    try {
      const comment = await commentsService.create(task.id, newComment.trim());
      setComments((prev) => [...prev, comment]);
      setNewComment('');
    } catch {
      toast.error('Error al crear el comentario');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="flex flex-col">
      <TaskCard task={task} onUpdate={onUpdate} showProject={showProject} members={members}/>

      <div className="border-x border-b border-border/50 rounded-b-lg bg-secondary/20 px-4 py-2">
        <Button
          variant="ghost"
          size="sm"
          onClick={handleToggle}
          className="h-7 gap-1.5 text-xs text-muted-foreground hover:text-foreground"
          disabled={loading}
        >
          {loading ? (
            <Loader2 className="h-3.5 w-3.5 animate-spin" />
          ) : (
            <MessageSquare className="h-3.5 w-3.5" />
          )}
          {comments.length > 0 ? `${comments.length} comentarios` : 'Ver comentarios'}
          {open ? <ChevronUp className="h-3.5 w-3.5" /> : <ChevronDown className="h-3.5 w-3.5" />}
        </Button>

        {open && (
          <div className="mt-2 flex flex-col gap-3 pb-2">
            {comments.length === 0 ? (
              <p className="text-xs text-muted-foreground">No hay comentarios aún.</p>
            ) : (
              comments.map((comment) => (
                <div key={comment.id} className="flex gap-2.5">
                  <Avatar className="h-6 w-6 shrink-0">
                    <AvatarFallback className="text-[10px]">
                      {comment.userName.slice(0, 2).toUpperCase()}
                    </AvatarFallback>
                  </Avatar>
                  <div className="flex flex-col gap-0.5">
                    <div className="flex items-center gap-2">
                      <span className="text-xs font-medium text-foreground">{comment.userName}</span>
                      <span className="text-[11px] text-muted-foreground">
                        {formatDistanceToNow(new Date(comment.createdAt), {
                          addSuffix: true,
                          locale: es,
                        })}
                      </span>
                    </div>
                    <p className="text-xs text-muted-foreground">{comment.content}</p>
                  </div>
                </div>
              ))
            )}

            <div className="mt-1 flex gap-2">
              <Textarea
                placeholder="Escribe un comentario..."
                value={newComment}
                onChange={(e) => setNewComment(e.target.value)}
                rows={1}
                className="min-h-0 resize-none text-xs py-1.5"
                onKeyDown={(e) => {
                  if (e.key === 'Enter' && !e.shiftKey) {
                    e.preventDefault();
                    handleSubmitComment();
                  }
                }}
              />
              <Button
                size="icon"
                className="h-8 w-8 shrink-0"
                onClick={handleSubmitComment}
                disabled={isSubmitting || !newComment.trim()}
              >
                {isSubmitting ? (
                  <Loader2 className="h-3.5 w-3.5 animate-spin" />
                ) : (
                  <Send className="h-3.5 w-3.5" />
                )}
              </Button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}