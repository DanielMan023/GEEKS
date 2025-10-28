import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

const ResetPasswordPage: React.FC = () => {
  const [form, setForm] = useState({ email: '', newPassword: '' });
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm(prev => ({ ...prev, [e.target.name]: e.target.value }));
    setSuccess(null);
    setError(null);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setSuccess(null);
    setError(null);
    try {
      const res = await fetch('/api/auth/reset-password', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(form)
      });
      const data = await res.json();
      if (res.ok && data.success !== false) {
        setSuccess('Contraseña restablecida correctamente. Ahora puedes iniciar sesión.');
        setTimeout(() => navigate('/login'), 1800);
      } else {
        setError(data.message || 'Error al restablecer la contraseña');
      }
    } catch {
      setError('No se pudo conectar al servidor');
    } finally {
      setLoading(false);
    }
  };

  return (
    <main className="min-h-screen bg-[#0C0C0C] flex">
      {/* Sección izquierda: Logo, igual que en login */}
      <section className="hidden lg:flex lg:w-1/2 bg-[#0C0C0C] items-center justify-center">
        <img 
          src="/geeks.png" 
          alt="GEEKS Logo" 
          className="w-[960px] h-[960px]"
        />
      </section>

      {/* Sección derecha: Panel de reset similar a login */}
      <section className="w-full lg:w-1/2 bg-[#424242] flex items-center justify-center">
        <div className="w-full max-w-lg px-8 py-12">
          <header className="text-left mb-8">
            <h1 className="text-4xl font-bold text-white mb-2">
              Restablecer contraseña
            </h1>
            <p className="text-gray-300">Ingresa tu correo y una nueva contraseña</p>
          </header>
          <form className="space-y-6" onSubmit={handleSubmit}>
            <div>
              <label htmlFor="email" className="block text-sm font-medium text-white mb-1">Correo electrónico</label>
              <input
                id="email"
                name="email"
                type="email"
                required
                value={form.email}
                onChange={handleChange}
                autoComplete="email"
                className="w-full px-3 py-3 bg-[#424242] border border-black rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent text-white placeholder-gray-400"
                placeholder="Ingresa tu correo"
              />
            </div>
            <div>
              <label htmlFor="newPassword" className="block text-sm font-medium text-white mb-1">Nueva contraseña</label>
              <input
                id="newPassword"
                name="newPassword"
                type="password"
                required
                minLength={6}
                value={form.newPassword}
                onChange={handleChange}
                autoComplete="new-password"
                className="w-full px-3 py-3 bg-[#424242] border border-black rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent text-white placeholder-gray-400"
                placeholder="Nueva contraseña"
              />
            </div>
            {success && <p className="text-green-400 text-center mt-2">{success}</p>}
            {error && <p className="text-red-400 text-center mt-2">{error}</p>}
            <button
              type="submit"
              className="w-full bg-green-600 hover:bg-green-700 text-white py-3 rounded-lg font-medium transition-colors"
              disabled={loading}
            >
              {loading ? 'Enviando...' : 'Restablecer contraseña'}
            </button>
          </form>
          <div className="text-center mt-6">
            <button onClick={() => navigate('/login')} className="text-blue-400 hover:text-blue-300 text-sm font-medium transition-colors">
              Volver al login
            </button>
          </div>
        </div>
      </section>
    </main>
  );
};

export default ResetPasswordPage;
