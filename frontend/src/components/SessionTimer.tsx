import React, { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { authService } from '../services/authService';

const SessionTimer: React.FC = () => {
  const { user } = useAuth();
  const [timeRemaining, setTimeRemaining] = useState<string>('');
  const [showWarning, setShowWarning] = useState(false);

  useEffect(() => {
    if (!user) return;

    const updateTimer = () => {
      const token = localStorage.getItem('auth-token');
      if (!token) return;

      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const expirationTime = payload.exp * 1000;
        const currentTime = Date.now();
        const remaining = expirationTime - currentTime;

        if (remaining <= 0) {
          setTimeRemaining('Sesión expirada');
          setShowWarning(false);
          return;
        }

        const minutes = Math.floor(remaining / 60000);
        const seconds = Math.floor((remaining % 60000) / 1000);
        
        setTimeRemaining(`${minutes}:${seconds.toString().padStart(2, '0')}`);

        // Mostrar advertencia cuando queden menos de 5 minutos
        if (remaining <= 5 * 60 * 1000) {
          setShowWarning(true);
        } else {
          setShowWarning(false);
        }
      } catch (error) {
        console.error('Error al calcular tiempo restante:', error);
      }
    };

    // Actualizar cada segundo
    const interval = setInterval(updateTimer, 1000);
    updateTimer(); // Ejecutar inmediatamente

    return () => clearInterval(interval);
  }, [user]);

  if (!user || !timeRemaining) return null;

  // No mostrar nada visualmente, solo verificar expiración en background
  return null;
};

export default SessionTimer;
