import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ReporteStockBajo } from '../models/reporte.models';

@Injectable({ providedIn: 'root' })
export class ReportesService {
  private readonly http = inject(HttpClient);

  getStockBajo(umbral = 5): Observable<ReporteStockBajo> {
    return this.http.get<ReporteStockBajo>('/api/reportes/stock-bajo', { params: { umbral } });
  }

  descargarPdfStockBajo(umbral = 5): Observable<Blob> {
    return this.http.get('/api/reportes/stock-bajo/pdf', {
      params: { umbral },
      responseType: 'blob'
    });
  }

  enviarPorEmail(umbral = 5): Observable<void> {
    return this.http.post<void>('/api/reportes/stock-bajo/enviar-email', null, { params: { umbral } });
  }
}
