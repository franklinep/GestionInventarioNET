import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  PaginatedResponse,
  Producto,
  ProductoFiltroRequest,
  ProductoPayload
} from '../models/producto.models';

@Injectable({ providedIn: 'root' })
export class ProductosService {
  private readonly http = inject(HttpClient);

  getPaginated(filtro: ProductoFiltroRequest): Observable<PaginatedResponse<Producto>> {
    let params = new HttpParams()
      .set('page', filtro.page)
      .set('pageSize', filtro.pageSize);

    if (filtro.search?.trim()) {
      params = params.set('search', filtro.search.trim());
    }

    if (filtro.categoriaIds?.length) {
      params = params.set('categoriaIds', filtro.categoriaIds.join(','));
    }

    if (typeof filtro.stockMaximo === 'number') {
      params = params.set('stockMaximo', filtro.stockMaximo);
    }

    return this.http.get<PaginatedResponse<Producto>>('/api/productos', { params });
  }

  create(payload: ProductoPayload): Observable<Producto> {
    return this.http.post<Producto>('/api/productos', this.toFormData(payload));
  }

  update(productoId: number, payload: ProductoPayload): Observable<Producto> {
    return this.http.put<Producto>(`/api/productos/${productoId}`, this.toFormData(payload));
  }

  delete(productoId: number): Observable<void> {
    return this.http.delete<void>(`/api/productos/${productoId}`);
  }

  private toFormData(payload: ProductoPayload): FormData {
    const body = new FormData();
    body.append('nombre', payload.nombre);
    body.append('precio', String(payload.precio));
    body.append('stock', String(payload.stock));
    body.append('categoriaId', String(payload.categoriaId));

    if (payload.descripcion) {
      body.append('descripcion', payload.descripcion);
    }

    if (payload.imagen) {
      body.append('imagen', payload.imagen);
    }

    return body;
  }
}
