export interface LoginDTO {
  email: string;
  password: string;
}

export interface RegisterDTO {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

export interface AuthResponse {
  message: string;
  data: {
    token: string;
    user: User;
  };
}

export interface Role {
  id: number;
  name: string;
  scope: string;
  description: string;
  state: string;
  users: any[];
  createdAtDateTime: string;
  updatedAtDateTime: string;
  idUserCreated: number | null;
  idUserUpdated: number | null;
}

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string | null;
  role: Role;
}

