import React from 'react';

interface ButtonProps {
  children?: React.ReactNode;
  content?: string;
  type?: 'button' | 'submit' | 'reset';
  width?: 'full' | 'auto';
  disabled?: boolean;
  className?: string;
  onClick?: () => void;
}

const Button: React.FC<ButtonProps> & {
  Submit: React.FC<ButtonProps>;
} = ({
  children,
  content,
  type = 'button',
  width = 'auto',
  disabled = false,
  className = '',
  onClick
}) => {
  const baseClasses = "inline-flex justify-center items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-green-500 hover:bg-green-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200";
  const widthClasses = width === 'full' ? 'w-full' : '';

  return (
    <button
      type={type}
      disabled={disabled}
      className={`${baseClasses} ${widthClasses} ${className}`}
      onClick={onClick}
    >
      {content || children}
    </button>
  );
};

Button.Submit = ({
  children,
  content,
  width = 'auto',
  disabled = false,
  className = '',
  onClick
}) => (
  <Button
    type="submit"
    width={width}
    disabled={disabled}
    className={className}
    onClick={onClick}
  >
    {content || children}
  </Button>
);

export { Button };
