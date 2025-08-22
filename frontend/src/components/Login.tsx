import React, { useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { Button, Input, PasswordInput } from './common';

interface LoginProps {
  onSwitchToRegister: () => void;
}

const Login: React.FC<LoginProps> = ({ onSwitchToRegister }) => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const { login } = useAuth();

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    if (!email || !password) {
      setError('Por favor completa todos los campos');
      setLoading(false);
      return;
    }

    try {
      const result = await login(email, password);
      if (result.success) {
        // El contexto ya maneja el estado del usuario
      } else {
        setError(result.message);
      }
    } catch (err) {
      setError('Error inesperado. Intenta de nuevo.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <main className="min-h-screen flex bg-[#0C0C0C]">
      {/* Left Column */}
      <section className="hidden lg:flex items-center justify-center flex-1 bg-[#0C0C0C]">
        <div className="text-center">
          <img src="/geeks.png" alt="Logo GEEKS Enterprise" className="w-[960px] h-[960px] mx-auto mb-8" />
        </div>
      </section>

      {/* Right Column (Login Form) */}
      <section className="flex items-center justify-center flex-1 px-4 sm:px-6 lg:flex-none lg:px-20 xl:px-24 bg-[#424242] border-2 border-gray-500 shadow-2xl rounded-lg mx-4 my-4">
        <form className="w-full max-w-sm space-y-6" onSubmit={handleSubmit}>
          <header>
            <h2 className="text-2xl font-extrabold text-[#E5E5E5]">
              Bienvenido a GEEKS
            </h2>
            <p className="mt-2 text-sm text-[#B3B3B3]">
              Por favor ingrese sus credenciales para ingresar
            </p>
          </header>

          <fieldset className="space-y-6">
            <Input
              id="email"
              name="email"
              type="email"
              label="Correo electrónico"
              placeholder="Correo electrónico"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              autoComplete="email"
              className="bg-[#424242] border-[#424242] text-white placeholder-white"
            />

            <PasswordInput
              id="password"
              name="password"
              label="Contraseña"
              placeholder="••••••••"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              autoComplete="current-password"
              className="bg-[#424242] border-[#424242] text-white placeholder-white"
            />
          </fieldset>

          <footer className="space-y-4">
            <nav className="flex justify-end">
              <a href="#" className="text-sm font-medium text-[#6B9DFF] hover:text-[#4685FF] transition-colors duration-200">
                ¿Olvidaste tu contraseña?
              </a>
            </nav>

            {error && <span className="text-red-500 text-sm text-center block">{error}</span>}

            <Button.Submit
              content={loading ? 'Cargando...' : 'Ingresar'}
              width="full"
              disabled={loading}
              className="!py-2.5"
            />
            <Button
              content="Registrarte"
              width="full"
              disabled={loading}
              className="!py-2.5 bg-blue-600 hover:bg-blue-700"
              onClick={onSwitchToRegister}
              type="button"
            />
          </footer>
        </form>
      </section>
    </main>
  );
};

export default Login;
