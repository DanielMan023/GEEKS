import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { Input, PasswordInput, Button } from './common';

export const Register = (): React.ReactElement => {
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: ''
  });
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [isLoading, setIsLoading] = useState(false);

  const { register } = useAuth();
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

    if (!formData.firstName.trim()) {
      newErrors.firstName = 'El nombre es requerido';
    }

    if (!formData.lastName.trim()) {
      newErrors.lastName = 'El apellido es requerido';
    }

    if (!formData.email.trim()) {
      newErrors.email = 'El email es requerido';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'El email no es válido';
    }

    if (!formData.password.trim()) {
      newErrors.password = 'La contraseña es requerida';
    } else if (formData.password.length < 6) {
      newErrors.password = 'La contraseña debe tener al menos 6 caracteres';
    }

    if (!formData.confirmPassword.trim()) {
      newErrors.confirmPassword = 'Confirma tu contraseña';
    } else if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = 'Las contraseñas no coinciden';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) return;

    setIsLoading(true);
    try {
      await register({
        firstName: formData.firstName,
        lastName: formData.lastName,
        email: formData.email,
        password: formData.password
      });
      navigate('/dashboard');
    } catch (error) {
      setErrors({ general: 'Error al crear la cuenta. Por favor, inténtalo de nuevo.' });
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

      {/* Right Section - Register Form */}
      <section className="w-full lg:w-1/2 bg-[#424242] flex items-center justify-center">
        <div className="w-full max-w-lg px-8 py-12">
          <header className="text-left mb-8">
            <h1 className="text-4xl font-bold text-white mb-2">
              Crear cuenta
            </h1>
            <p className="text-gray-300">
              Complete el formulario para unirse a nuestra plataforma
            </p>
          </header>

          <form onSubmit={handleSubmit} className="space-y-6">
            {errors.general && (
              <div className="bg-red-500 text-white p-3 rounded-lg text-sm text-center">
                {errors.general}
              </div>
            )}

            <div className="grid grid-cols-2 gap-4">
              <div>
                <Input
                  id="firstName"
                  name="firstName"
                  type="text"
                  label="Nombre"
                  value={formData.firstName}
                  onChange={handleChange}
                  required
                  autoComplete="given-name"
                  className={errors.firstName ? 'border-red-500' : ''}
                />
                {errors.firstName && (
                  <p className="text-red-500 text-sm mt-1">{errors.firstName}</p>
                )}
              </div>

              <div>
                <Input
                  id="lastName"
                  name="lastName"
                  type="text"
                  label="Apellido"
                  value={formData.lastName}
                  onChange={handleChange}
                  required
                  autoComplete="family-name"
                  className={errors.lastName ? 'border-red-500' : ''}
                />
                {errors.lastName && (
                  <p className="text-red-500 text-sm mt-1">{errors.lastName}</p>
                )}
              </div>
            </div>

            <Input
              id="email"
              name="email"
              type="email"
              label="Correo o usuario"
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
              autoComplete="new-password"
              showMinLength={true}
              className={errors.password ? 'border-red-500' : ''}
            />
            {errors.password && (
              <p className="text-red-500 text-sm mt-1">{errors.password}</p>
            )}

            <PasswordInput
              id="confirmPassword"
              name="confirmPassword"
              label="Confirmar Contraseña"
              value={formData.confirmPassword}
              onChange={handleChange}
              required
              autoComplete="new-password"
              className={errors.confirmPassword ? 'border-red-500' : ''}
            />
            {errors.confirmPassword && (
              <p className="text-red-500 text-sm mt-1">{errors.confirmPassword}</p>
            )}

            <Button.Submit
              content="Crear Cuenta"
              width="full"
              disabled={isLoading}
              className="bg-green-600 hover:bg-green-700"
            >
              {isLoading ? 'Creando cuenta...' : 'Crear Cuenta'}
            </Button.Submit>
          </form>

          <div className="mt-6 text-center">
            <p className="text-[#6C6975]">
              ¿Ya tienes una cuenta?{' '}
              <Link 
                to="/login" 
                className="text-blue-400 hover:text-blue-300 font-medium transition-colors"
              >
                Inicia sesión aquí
              </Link>
            </p>
          </div>
        </div>
      </section>
    </main>
  );
};

export default Register;
