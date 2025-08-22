import React, { forwardRef } from 'react';

interface InputProps {
  id: string;
  name: string;
  type?: 'text' | 'email' | 'password' | 'tel' | 'number';
  label: string;
  placeholder?: string;
  value: string;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  required?: boolean;
  autoComplete?: string;
  disabled?: boolean;
  className?: string;
}

const Input = forwardRef<HTMLInputElement, InputProps>(({
  id,
  name,
  type = 'text',
  label,
  placeholder,
  value,
  onChange,
  required = false,
  autoComplete,
  disabled = false,
  className = ''
}, ref) => {
  return (
    <div className="relative">
      <input
        ref={ref}
        type={type}
        id={id}
        name={name}
        autoComplete={autoComplete}
        value={value}
        onChange={onChange}
        className={`w-full px-3 py-4 border border-gray-300 rounded-lg focus:outline-none focus:border-green-500 focus:ring-1 focus:ring-green-500 transition-all duration-200 peer placeholder-transparent text-white [&:-webkit-autofill]:bg-[#424242] [&:-webkit-autofill]:text-white [&:-webkit-autofill]:shadow-[0_0_0_30px_#424242_inset] [&:-webkit-autofill]:-webkit-text-fill-color-white ${className}`}
        placeholder=" "
        required={required}
        disabled={disabled}
      />
      <label
        htmlFor={id}
        className="absolute left-3 -top-2.5 px-2 text-sm text-white bg-[#424242] peer-placeholder-shown:text-base peer-placeholder-shown:text-gray-400 peer-placeholder-shown:top-4 peer-focus:-top-2.5 peer-focus:text-sm peer-focus:text-green-500 transition-all duration-200"
      >
        {label}
      </label>
    </div>
  );
});

Input.displayName = 'Input';

export { Input };
