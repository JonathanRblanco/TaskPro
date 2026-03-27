'use client';

import { createContext, useContext, useEffect, useState, useCallback, ReactNode } from 'react';
import { useRouter } from 'next/navigation';
import Cookies from 'js-cookie';
import { User, AuthContextType } from '@/types';
import { authService } from '@/lib/api';
import { mutate } from 'swr';

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const router = useRouter();

  useEffect(() => {
    const initAuth = () => {
      const storedToken = Cookies.get('token');
      const storedUser = Cookies.get('user');

      if (storedToken && storedUser) {
        try {
          setToken(storedToken);
          setUser(JSON.parse(storedUser));
        } catch {
          Cookies.remove('token');
          Cookies.remove('user');
        }
      }
      setIsLoading(false);
    };

    initAuth();
  }, []);

  const login = useCallback(async (email: string, password: string) => {
    setIsLoading(true);
    try {
      const response = await authService.login({ email, password });
      const user: User = {
        nombre: response.fullName,
        email: response.email,
        rol: response.role,
        fechaCreacion: response.expiresAt,
      };
      Cookies.set('token', response.token, { expires: 7, secure: true, sameSite: 'strict' });
      Cookies.set('user', JSON.stringify(user), { expires: 7, secure: true, sameSite: 'strict' });
      
      setToken(response.token);
      setUser(user);
      
      router.push('/dashboard');
    } finally {
      setIsLoading(false);
    }
  }, [router]);

  const logout = useCallback(() => {
    Cookies.remove('token');
    Cookies.remove('user');
    setToken(null);
    setUser(null);
    mutate(() => true, undefined, { revalidate: true });
    authService.logout();
    router.push('/login');
  }, [router]);

  const value: AuthContextType = {
    user,
    token,
    isLoading,
    isAuthenticated: !!token && !!user,
    login,
    logout,
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}

export default AuthContext;
