import { useState } from 'react';

interface ConfirmOptions {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  type?: 'danger' | 'warning' | 'info';
}

export const useConfirm = () => {
  const [confirmState, setConfirmState] = useState<{
    isOpen: boolean;
    options: ConfirmOptions | null;
    onConfirm: (() => void) | null;
  }>({
    isOpen: false,
    options: null,
    onConfirm: null,
  });

  const showConfirm = (options: ConfirmOptions, onConfirm: () => void) => {
    setConfirmState({
      isOpen: true,
      options,
      onConfirm,
    });
  };

  const handleConfirm = () => {
    if (confirmState.onConfirm) {
      confirmState.onConfirm();
    }
    setConfirmState({
      isOpen: false,
      options: null,
      onConfirm: null,
    });
  };

  const handleCancel = () => {
    setConfirmState({
      isOpen: false,
      options: null,
      onConfirm: null,
    });
  };

  return {
    isOpen: confirmState.isOpen,
    options: confirmState.options,
    showConfirm,
    handleConfirm,
    handleCancel,
  };
};
