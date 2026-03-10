import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { AuthResponse, AuthUser, LoginRequest, RegisterRequest } from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly tokenKey = 'gi_token';
  private readonly userKey = 'gi_user';

  readonly currentUser = signal<AuthUser | null>(this.restoreUser());

  login(payload: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>('/api/auth/login', payload).pipe(tap((response) => this.persistSession(response)));
  }

  register(payload: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>('/api/auth/register', payload)
      .pipe(tap((response) => this.persistSession(response)));
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this.currentUser.set(null);
  }

  isAuthenticated(): boolean {
    return this.currentUser() !== null;
  }

  token(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isAdmin(): boolean {
    return this.currentUser()?.role.toLowerCase() === 'admin';
  }

  private persistSession(response: AuthResponse): void {
    const role = this.extractRole(response.accessToken);
    const user: AuthUser = {
      usuarioId: response.usuarioId,
      nombre: response.nombre,
      correo: response.correo,
      role,
      token: response.accessToken,
      expiresAtUtc: response.expiresAtUtc
    };

    localStorage.setItem(this.tokenKey, response.accessToken);
    localStorage.setItem(this.userKey, JSON.stringify(user));
    this.currentUser.set(user);
  }

  private restoreUser(): AuthUser | null {
    const token = localStorage.getItem(this.tokenKey);
    const rawUser = localStorage.getItem(this.userKey);

    if (!token || !rawUser) {
      return null;
    }

    try {
      const user = JSON.parse(rawUser) as AuthUser;
      if (this.isTokenExpired(token)) {
        this.logout();
        return null;
      }

      return { ...user, token };
    } catch {
      this.logout();
      return null;
    }
  }

  private isTokenExpired(token: string): boolean {
    const payload = this.parseTokenPayload(token);
    if (!payload) {
      return true;
    }

    const exp = payload['exp'];
    if (typeof exp !== 'number') {
      return false;
    }

    return Date.now() >= exp * 1000;
  }

  private extractRole(token: string): string {
    const payload = this.parseTokenPayload(token);
    if (!payload) {
      return 'empleado';
    }

    const roleClaim =
      payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? payload['role'];

    return typeof roleClaim === 'string' ? roleClaim : 'empleado';
  }

  private parseTokenPayload(token: string): Record<string, unknown> | null {
    const parts = token.split('.');
    if (parts.length !== 3) {
      return null;
    }

    try {
      const normalized = parts[1].replace(/-/g, '+').replace(/_/g, '/');
      const padded = normalized.padEnd(normalized.length + ((4 - (normalized.length % 4)) % 4), '=');
      const decoded = atob(padded);
      return JSON.parse(decoded) as Record<string, unknown>;
    } catch {
      return null;
    }
  }
}
