import React, { createContext, useContext, useState, ReactNode } from 'react';

interface LoadingContextType {
  isLoading: boolean;
  loadingMessage: string;
  setLoading: (loading: boolean, message?: string) => void;
  withLoading: <T>(fn: () => Promise<T>, message?: string) => Promise<T>;
}

const LoadingContext = createContext<LoadingContextType | undefined>(undefined);

export const useLoading = () => {
  const context = useContext(LoadingContext);
  if (context === undefined) {
    throw new Error('useLoading must be used within a LoadingProvider');
  }
  return context;
};

interface LoadingProviderProps {
  children: ReactNode;
}

export const LoadingProvider: React.FC<LoadingProviderProps> = ({ children }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [loadingMessage, setLoadingMessage] = useState('Cargando...');

  const setLoading = (loading: boolean, message: string = 'Cargando...') => {
    setIsLoading(loading);
    setLoadingMessage(message);
  };

  const withLoading = async <T,>(fn: () => Promise<T>, message: string = 'Cargando...'): Promise<T> => {
    setLoading(true, message);
    try {
      const result = await fn();
      return result;
    } finally {
      setLoading(false);
    }
  };

  const value: LoadingContextType = {
    isLoading,
    loadingMessage,
    setLoading,
    withLoading
  };

  return (
    <LoadingContext.Provider value={value}>
      {children}
    </LoadingContext.Provider>
  );
};
