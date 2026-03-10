export interface ProductoStockBajo {
  productoId: number;
  nombre: string;
  categoriaNombre: string;
  stockActual: number;
  precio: number;
  nivel: string;
  ultimaActualizacion: string;
}

export interface ReporteStockBajo {
  fechaGeneracion: string;
  totalProductos: number;
  productosCriticos: number;
  valorAfectado: number;
  productos: ProductoStockBajo[];
}
