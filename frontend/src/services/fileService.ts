import axios from 'axios';

const API_BASE_URL = 'http://localhost:5000/api';

export interface UploadImageResponse {
  success: boolean;
  imageUrl: string;
  fileName: string;
  message: string;
}

export const fileService = {
  async uploadImage(file: File): Promise<UploadImageResponse> {
    const formData = new FormData();
    formData.append('file', file);

    try {
      const response = await axios.post<UploadImageResponse>(
        `${API_BASE_URL}/file/upload-image`,
        formData,
        {
          headers: {
            'Content-Type': 'multipart/form-data',
          },
          withCredentials: true,
        }
      );

      return response.data;
    } catch (error: any) {
      if (error.response?.data) {
        throw new Error(error.response.data.message || 'Error al subir la imagen');
      }
      throw new Error('Error de conexión al subir la imagen');
    }
  },

  async deleteImage(fileName: string): Promise<void> {
    try {
      await axios.delete(`${API_BASE_URL}/file/delete-image/${fileName}`, {
        withCredentials: true,
      });
    } catch (error: any) {
      if (error.response?.data) {
        throw new Error(error.response.data.message || 'Error al eliminar la imagen');
      }
      throw new Error('Error de conexión al eliminar la imagen');
    }
  },

  // Función para obtener la URL completa de la imagen
  getImageUrl(imagePath: string): string {
    if (!imagePath) return '';
    
    // Si ya es una URL completa, retornarla tal como está
    if (imagePath.startsWith('http://') || imagePath.startsWith('https://')) {
      return imagePath;
    }
    
    // Si es una ruta relativa, construir la URL completa
    return `http://localhost:5000${imagePath}`;
  }
};
