export interface LoginRequest {
  correo: string;
  password: string;
}

export interface RegisterRequest {
  nombre: string;
  correo: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  expiresAtUtc: string;
  usuarioId: number;
  nombre: string;
  correo: string;
}

export interface AuthUser {
  usuarioId: number;
  nombre: string;
  correo: string;
  role: string;
  token: string;
  expiresAtUtc: string;
}
