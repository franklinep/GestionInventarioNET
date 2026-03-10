export interface Producto {
  productoId: number;
  nombre: string;
  descripcion?: string | null;
  imagenUrl?: string | null;
  precio: number;
  stock: number;
  categoriaId: number;
  categoriaNombre: string;
  createdAt: string;
  updatedAt: string;
}

export interface ProductoPayload {
  nombre: string;
  descripcion?: string | null;
  precio: number;
  stock: number;
  categoriaId: number;
  imagen?: File | null;
}

export interface ProductoFiltroRequest {
  search?: string;
  categoriaIds?: number[];
  stockMaximo?: number | null;
  page: number;
  pageSize: number;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalItems: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
