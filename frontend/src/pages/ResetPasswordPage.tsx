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
      // Supón que existe una ruta: POST /api/auth/reset-password con {email, newPassword}
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
    <main className="min-h-screen flex items-center justify-center bg-[#0C0C0C]">
      <section className="bg-white p-8 rounded-lg shadow-md w-full max-w-md">
        <h1 className="text-2xl font-bold mb-6 text-gray-900 text-center">Restablecer contraseña</h1>
        <form className="space-y-6" onSubmit={handleSubmit}>
          <div>
            <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-1">Correo electrónico</label>
            <input
              id="email"
              name="email"
              type="email"
              required
              value={form.email}
              onChange={handleChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-green-500 focus:border-green-500"
            />
          </div>
          <div>
            <label htmlFor="newPassword" className="block text-sm font-medium text-gray-700 mb-1">Nueva contraseña</label>
            <input
              id="newPassword"
              name="newPassword"
              type="password"
              required
              minLength={6}
              value={form.newPassword}
              onChange={handleChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-green-500 focus:border-green-500"
            />
          </div>
          <button
            type="submit"
            className="w-full bg-green-600 text-white py-3 rounded-lg hover:bg-green-700 transition-colors font-medium"
            disabled={loading}
          >
            {loading ? 'Enviando...' : 'Restablecer contraseña'}
          </button>
        </form>
        {success && <p className="text-green-600 text-center mt-4">{success}</p>}
        {error && <p className="text-red-600 text-center mt-4">{error}</p>}
        <div className="text-center mt-4">
          <button onClick={() => navigate('/login')} className="text-blue-400 hover:text-blue-600 text-sm">Volver al login</button>
        </div>
      </section>
    </main>
  );
};

export default ResetPasswordPage;
