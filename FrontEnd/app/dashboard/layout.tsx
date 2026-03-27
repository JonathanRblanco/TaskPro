'use client';

import { AuthGuard } from '@/components/auth-guard';
import { Sidebar } from '@/components/dashboard/sidebar';

export default function DashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <AuthGuard>
      <div className="min-h-screen bg-background">
        <Sidebar />
        <main className="md:pl-64">
          <div className="min-h-screen p-4 md:p-8">
            {children}
          </div>
        </main>
      </div>
    </AuthGuard>
  );
}
