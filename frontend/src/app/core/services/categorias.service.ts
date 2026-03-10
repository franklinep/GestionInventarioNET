import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Categoria } from '../models/categoria.models';

@Injectable({ providedIn: 'root' })
export class CategoriasService {
  private readonly http = inject(HttpClient);

  getActivas(): Observable<Categoria[]> {
    return this.http.get<Categoria[]>('/api/categorias');
  }

  crear(nombre: string): Observable<Categoria> {
    return this.http.post<Categoria>('/api/categorias', { nombre });
  }

  actualizar(id: number, nombre: string): Observable<Categoria> {
    return this.http.put<Categoria>(`/api/categorias/${id}`, { nombre });
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`/api/categorias/${id}`);
  }
}
