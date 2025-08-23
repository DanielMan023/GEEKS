import React, { useState } from 'react';
import { EyeIcon } from '../icons/EyeIcon';
import { EyeSlashIcon } from '../icons/EyeSlashIcon';

interface PasswordInputProps {
  id: string;
  name: string;
  label: string;
  placeholder?: string;
  value: string;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  required?: boolean;
  autoComplete?: string;
  disabled?: boolean;
  className?: string;
  showMinLength?: boolean;
}

const PasswordInput: React.FC<PasswordInputProps> = ({
  id,
  name,
  label,
  placeholder = '••••••••',
  value,
  onChange,
  required = false,
  autoComplete = 'current-password',
  disabled = false,
  className = '',
  showMinLength = false
}) => {
  const [showPassword, setShowPassword] = useState(false);

  const togglePassword = () => {
    setShowPassword(prev => !prev);
  };

  return (
    <div className="relative">
      <input
        type={showPassword ? 'text' : 'password'}
        id={id}
        name={name}
        autoComplete={autoComplete}
        value={value}
        onChange={onChange}
        className={`w-full px-3 py-4 bg-[#424242] border border-black rounded-lg focus:outline-none focus:border-green-500 focus:ring-1 focus:ring-green-500 transition-all duration-200 peer placeholder-transparent pr-10 text-white [&:-webkit-autofill]:!bg-[#424242] [&:-webkit-autofill]:!text-white [&:-webkit-autofill]:!shadow-[0_0_0_30px_#424242_inset] [&:-webkit-autofill]:!-webkit-text-fill-color-white [&:-webkit-autofill]:!color-white [&:-webkit-autofill]:!border-black ${className}`}
        placeholder=" "
        required={required}
        disabled={disabled}
        style={{
          WebkitTextFillColor: 'white !important',
          color: 'white !important'
        }}
      />
      <label
        htmlFor={id}
        className="absolute left-3 -top-2.5 px-2 text-sm text-white bg-[#424242] peer-placeholder-shown:text-base peer-placeholder-shown:text-gray-400 peer-placeholder-shown:top-4 peer-focus:-top-2.5 peer-focus:text-sm peer-focus:text-green-500 transition-all duration-200"
      >
        {label}
      </label>
      <button
        type="button"
        onClick={togglePassword}
        className="absolute right-3 top-4 text-gray-400 hover:text-gray-600 transition-colors duration-200"
        tabIndex={-1}
      >
        {showPassword ? <EyeIcon /> : <EyeSlashIcon />}
      </button>
      {showMinLength && (
        <p className="text-xs text-gray-500 mt-1 ml-1">
          Mínimo 6 caracteres
        </p>
      )}
    </div>
  );
};

export { PasswordInput };
