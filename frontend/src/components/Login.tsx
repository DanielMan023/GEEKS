import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { Input, PasswordInput, Button } from './common';

export const Login = (): React.ReactElement => {
  const [formData, setFormData] = useState({
    email: '',
    password: ''
  });
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [isLoading, setIsLoading] = useState(false);

  const { login } = useAuth();
  const navigate = useNavigate();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    // Limpiar error del campo cuando el usuario empiece a escribir
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.email.trim()) {
      newErrors.email = 'El email es requerido';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'El email no es válido';
    }

    if (!formData.password.trim()) {
      newErrors.password = 'La contraseña es requerida';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) return;

    setIsLoading(true);
    try {
      await login(formData.email, formData.password);
      navigate('/dashboard');
    } catch (error) {
      setErrors({ general: 'Credenciales inválidas. Por favor, inténtalo de nuevo.' });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <main className="min-h-screen bg-[#0C0C0C] flex">
      {/* Left Section - Logo */}
      <section className="hidden lg:flex lg:w-1/2 bg-[#0C0C0C] items-center justify-center">
        <img 
          src="/geeks.png" 
          alt="GEEKS Logo" 
          className="w-[960px] h-[960px]"
        />
      </section>

      {/* Right Section - Login Form */}
      <section className="w-full lg:w-1/2 bg-[#424242] border-2 border-gray-500 shadow-2xl rounded-lg mx-4 my-4 flex items-center justify-center">
        <div className="w-full max-w-md px-8 py-12">
          <header className="text-center mb-8">
            <h1 className="text-4xl font-bold text-white mb-2">
              GEEKS ENTERPRISE
            </h1>
            <h2 className="text-2xl font-semibold text-white mb-2">
              Bienvenido a GEEKS
            </h2>
            <p className="text-gray-300">
              Por favor ingrese sus credenciales para ingresar
            </p>
          </header>

          <form onSubmit={handleSubmit} className="space-y-6">
            {errors.general && (
              <div className="bg-red-500 text-white p-3 rounded-lg text-sm text-center">
                {errors.general}
              </div>
            )}

            <Input
              id="email"
              name="email"
              type="email"
              label="Email"
              value={formData.email}
              onChange={handleChange}
              required
              autoComplete="email"
              className={errors.email ? 'border-red-500' : ''}
            />
            {errors.email && (
              <p className="text-red-500 text-sm mt-1">{errors.email}</p>
            )}

            <PasswordInput
              id="password"
              name="password"
              label="Contraseña"
              value={formData.password}
              onChange={handleChange}
              required
              autoComplete="current-password"
              className={errors.password ? 'border-red-500' : ''}
            />
            {errors.password && (
              <p className="text-red-500 text-sm mt-1">{errors.password}</p>
            )}

            <Button.Submit
              content="Iniciar Sesión"
              width="full"
              disabled={isLoading}
              className="bg-green-600 hover:bg-green-700"
            >
              {isLoading ? 'Iniciando sesión...' : 'Iniciar Sesión'}
            </Button.Submit>
          </form>

          <div className="mt-6 text-center">
            <p className="text-gray-300">
              ¿No tienes una cuenta?{' '}
              <Link 
                to="/register" 
                className="text-green-400 hover:text-green-300 font-medium transition-colors"
              >
                Regístrate aquí
              </Link>
            </p>
          </div>
        </div>
      </section>
    </main>
  );
};

export default Login;
