import React, { useState, useRef } from 'react';
import { Upload, X, Image as ImageIcon } from 'lucide-react';
import { fileService } from '../../services/fileService';
import PlaceholderImage from './PlaceholderImage';

interface ImageUploadProps {
  onImageUploaded: (imageUrl: string) => void;
  currentImage?: string;
  className?: string;
}

const ImageUpload: React.FC<ImageUploadProps> = ({ 
  onImageUploaded, 
  currentImage, 
  className = '' 
}) => {
  const [isUploading, setIsUploading] = useState(false);
  const [uploadError, setUploadError] = useState<string | null>(null);
  const [previewImage, setPreviewImage] = useState<string | null>(currentImage || null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileSelect = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    // Validar tipo de archivo
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
    if (!allowedTypes.includes(file.type)) {
      setUploadError('Solo se permiten archivos de imagen (JPG, PNG, GIF, WEBP)');
      return;
    }

    // Validar tamaño (máximo 5MB)
    if (file.size > 5 * 1024 * 1024) {
      setUploadError('El archivo es demasiado grande. Máximo 5MB');
      return;
    }

    try {
      setIsUploading(true);
      setUploadError(null);

      // Crear preview local
      const reader = new FileReader();
      reader.onload = (e) => {
        setPreviewImage(e.target?.result as string);
      };
      reader.readAsDataURL(file);

      // Subir imagen al servidor
      const response = await fileService.uploadImage(file);
      
      if (response.success) {
        onImageUploaded(response.imageUrl);
        setUploadError(null);
      } else {
        throw new Error(response.message);
      }
    } catch (error: any) {
      setUploadError(error.message || 'Error al subir la imagen');
      setPreviewImage(currentImage || null);
    } finally {
      setIsUploading(false);
    }
  };

  const handleRemoveImage = () => {
    setPreviewImage(null);
    onImageUploaded('');
    if (fileInputRef.current) {
      fileInputRef.current.value = '';
    }
  };

  const handleClickUpload = () => {
    fileInputRef.current?.click();
  };

  return (
    <div className={`space-y-3 ${className}`}>
      <label className="block text-sm font-medium text-gray-700 mb-1">
        Imagen del Producto
      </label>
      
      {/* Área de subida */}
      <div className="space-y-3">
        {/* Preview de imagen actual */}
        {previewImage && (
          <div className="relative inline-block">
            {previewImage.startsWith('data:') ? (
              <img
                src={previewImage}
                alt="Preview"
                className="w-24 h-24 sm:w-32 sm:h-32 object-cover rounded-lg border border-gray-300"
              />
            ) : (
              <PlaceholderImage 
                width={128} 
                height={128} 
                text="Preview" 
                className="w-24 h-24 sm:w-32 sm:h-32 rounded-lg border border-gray-300"
              />
            )}
            <button
              type="button"
              onClick={handleRemoveImage}
              className="absolute -top-2 -right-2 bg-red-500 text-white rounded-full p-1 hover:bg-red-600 transition-colors"
              title="Eliminar imagen"
            >
              <X size={14} className="sm:hidden" />
              <X size={16} className="hidden sm:block" />
            </button>
          </div>
        )}

        {/* Botón de subida */}
        {!previewImage && (
          <div
            onClick={handleClickUpload}
            className="w-24 h-24 sm:w-32 sm:h-32 border-2 border-dashed border-gray-300 rounded-lg flex flex-col items-center justify-center cursor-pointer hover:border-green-400 hover:bg-green-50 transition-colors"
          >
            <Upload size={20} className="text-gray-400 mb-1 sm:mb-2 sm:hidden" />
            <Upload size={24} className="text-gray-400 mb-2 hidden sm:block" />
            <span className="text-xs sm:text-sm text-gray-500 text-center px-2">
              Click para subir
            </span>
          </div>
        )}

        {/* Botón de cambiar imagen */}
        {previewImage && (
          <button
            type="button"
            onClick={handleClickUpload}
            className="flex items-center gap-2 px-3 py-2 text-xs sm:text-sm text-green-600 hover:text-green-700 hover:bg-green-50 rounded-lg transition-colors w-full sm:w-auto justify-center sm:justify-start"
          >
            <ImageIcon size={14} className="sm:hidden" />
            <ImageIcon size={16} className="hidden sm:block" />
            Cambiar imagen
          </button>
        )}

        {/* Input de archivo oculto */}
        <input
          ref={fileInputRef}
          type="file"
          accept="image/*"
          onChange={handleFileSelect}
          className="hidden"
        />
      </div>

      {/* Estados de carga y error */}
      {isUploading && (
        <div className="flex items-center gap-2 text-sm text-blue-600">
          <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-blue-600"></div>
          Subiendo imagen...
        </div>
      )}

      {uploadError && (
        <div className="text-sm text-red-600 bg-red-50 p-2 rounded-lg">
          {uploadError}
        </div>
      )}

      {/* Información de ayuda */}
      <div className="text-xs text-gray-500">
        Formatos soportados: JPG, PNG, GIF, WEBP. Tamaño máximo: 5MB
      </div>
    </div>
  );
};

export default ImageUpload;
