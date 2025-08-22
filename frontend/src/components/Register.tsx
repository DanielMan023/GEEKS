import React, { useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { Button, Input, PasswordInput } from './common';

interface RegisterProps {
  onSwitchToLogin: () => void;
}

const Register: React.FC<RegisterProps> = ({ onSwitchToLogin }) => {
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: ''
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const { register } = useAuth();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const validateForm = () => {
    if (!formData.firstName || !formData.lastName || !formData.email || !formData.password) {
      setError('Por favor completa todos los campos');
      return false;
    }

    if (formData.password.length < 6) {
      setError('La contraseña debe tener al menos 6 caracteres');
      return false;
    }

    if (formData.password !== formData.confirmPassword) {
      setError('Las contraseñas no coinciden');
      return false;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(formData.email)) {
      setError('Por favor ingresa un email válido');
      return false;
    }

    return true;
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    if (!validateForm()) {
      setLoading(false);
      return;
    }

    try {
      const result = await register({
        firstName: formData.firstName,
        lastName: formData.lastName,
        email: formData.email,
        password: formData.password
      });

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

      {/* Right Column (Register Form) */}
      <section className="flex items-center justify-center flex-1 px-4 sm:px-6 lg:flex-none lg:px-20 xl:px-24 bg-[#424242] border-2 border-gray-500 shadow-2xl rounded-lg mx-4 my-4">
        <form className="w-full max-w-sm space-y-6" onSubmit={handleSubmit}>
          <header>
            <h2 className="text-2xl font-extrabold text-[#E5E5E5]">
              Crear cuenta en GEEKS
            </h2>
            <p className="mt-2 text-sm text-[#B3B3B3]">
              Complete el formulario para unirse a nuestra plataforma
            </p>
          </header>

          <fieldset className="space-y-6">
            {/* Name Fields */}
            <div className="grid grid-cols-2 gap-4">
              <Input
                id="firstName"
                name="firstName"
                type="text"
                label="Nombre"
                placeholder="Nombre"
                value={formData.firstName}
                onChange={handleChange}
                required
                autoComplete="given-name"
                className="bg-[#424242] border-[#424242] text-white placeholder-white"
              />

              <Input
                id="lastName"
                name="lastName"
                type="text"
                label="Apellido"
                placeholder="Apellido"
                value={formData.lastName}
                onChange={handleChange}
                required
                autoComplete="family-name"
                className="bg-[#424242] border-[#424242] text-white placeholder-white"
              />
            </div>

            <Input
              id="email"
              name="email"
              type="email"
              label="Correo electrónico"
              placeholder="Correo electrónico"
              value={formData.email}
              onChange={handleChange}
              required
              autoComplete="email"
              className="bg-[#424242] border-[#424242] text-white placeholder-white"
            />

            <PasswordInput
              id="password"
              name="password"
              label="Contraseña"
              placeholder="••••••••"
              value={formData.password}
              onChange={handleChange}
              required
              autoComplete="new-password"
              showMinLength={true}
              className="bg-[#424242] border-[#424242] text-white placeholder-white"
            />

            <PasswordInput
              id="confirmPassword"
              name="confirmPassword"
              label="Confirmar contraseña"
              placeholder="••••••••"
              value={formData.confirmPassword}
              onChange={handleChange}
              required
              autoComplete="new-password"
              className="bg-[#424242] border-[#424242] text-white placeholder-white"
            />
          </fieldset>

          <footer className="space-y-4">
            {error && <span className="text-red-500 text-sm text-center block">{error}</span>}

            <Button.Submit
              content={loading ? 'Creando cuenta...' : 'Crear cuenta'}
              width="full"
              disabled={loading}
              className="!py-2.5"
            />
            
            <div className="text-center">
              <p className="text-sm text-[#6C6975]">
                ¿Ya tienes una cuenta?{' '}
                <button
                  type="button"
                  onClick={onSwitchToLogin}
                  className="text-[#6B9DFF] hover:text-[#4685FF] font-medium transition-colors duration-200"
                >
                  Inicia sesión aquí
                </button>
              </p>
            </div>
          </footer>
        </form>
      </section>
    </main>
  );
};

export default Register;
