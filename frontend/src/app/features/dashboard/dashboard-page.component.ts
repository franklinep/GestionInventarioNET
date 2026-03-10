import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { forkJoin } from 'rxjs';
import { DashboardStats } from '../../core/models/dashboard.models';
import { Producto } from '../../core/models/producto.models';
import { DashboardService } from '../../core/services/dashboard.service';
import { ProductosService } from '../../core/services/productos.service';

@Component({
  selector: 'app-dashboard-page',
  imports: [CommonModule],
  templateUrl: './dashboard-page.component.html',
  styleUrl: './dashboard-page.component.scss'
})
export class DashboardPageComponent implements OnInit {
  private readonly dashboardService = inject(DashboardService);
  private readonly productosService = inject(ProductosService);

  readonly loading = signal(true);
  readonly error = signal('');
  readonly stats = signal<DashboardStats | null>(null);
  readonly productos = signal<Producto[]>([]);

  ngOnInit(): void {
    this.loadData();
  }

  private loadData(): void {
    this.loading.set(true);
    this.error.set('');

    forkJoin({
      stats: this.dashboardService.getStats(),
      productos: this.productosService.getPaginated({ page: 1, pageSize: 5 })
    }).subscribe({
      next: ({ stats, productos }) => {
        this.stats.set(stats);
        this.productos.set(productos.items);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('No se pudo cargar el dashboard');
        this.loading.set(false);
      }
    });
  }
}
