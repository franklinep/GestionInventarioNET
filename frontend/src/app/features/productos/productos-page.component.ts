import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { Categoria } from '../../core/models/categoria.models';
import { Producto, ProductoPayload } from '../../core/models/producto.models';
import { AuthService } from '../../core/services/auth.service';
import { CategoriasService } from '../../core/services/categorias.service';
import { ProductosService } from '../../core/services/productos.service';
import { ReportesService } from '../../core/services/reportes.service';

@Component({
  selector: 'app-productos-page',
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './productos-page.component.html',
  styleUrl: './productos-page.component.scss'
})
export class ProductosPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly productosService = inject(ProductosService);
  private readonly categoriasService = inject(CategoriasService);
  private readonly reportesService = inject(ReportesService);
  private readonly authService = inject(AuthService);

  readonly loading = signal(true);
  readonly actionLoading = signal(false);
  readonly error = signal('');
  readonly feedback = signal('');

  categorias: Categoria[] = [];
  productos: Producto[] = [];

  search = '';
  page = 1;
  pageSize = 10;
  totalPages = 1;
  totalItems = 0;

  selectedCategoriaIds: number[] = [];
  stockMaximo: number | null = null;

  showFilterModal = false;
  filterCategoryEnabled = false;
  filterStockEnabled = false;
  filterCategoriaIdsDraft: number[] = [];
  filterStockDraft = 5;

  showProductoModal = false;
  editingProducto: Producto | null = null;
  selectedImage: File | null = null;

  showEmailModal = false;
  emailUmbral = 5;

  readonly form = this.fb.nonNullable.group({
    nombre: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(150)]],
    descripcion: ['', [Validators.maxLength(1000)]],
    precio: [1, [Validators.required, Validators.min(0.01)]],
    stock: [0, [Validators.required, Validators.min(0)]],
    categoriaId: [0, [Validators.required, Validators.min(1)]]
  });

  ngOnInit(): void {
    this.loadInitialData();
  }

  get isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  get pages(): number[] {
    const maxVisible = 5;
    const start = Math.max(1, this.page - 2);
    const end = Math.min(this.totalPages, start + maxVisible - 1);
    const fixedStart = Math.max(1, end - maxVisible + 1);

    return Array.from({ length: end - fixedStart + 1 }, (_, index) => fixedStart + index);
  }

  loadProductos(): void {
    this.loading.set(true);
    this.error.set('');

    this.productosService
      .getPaginated({
        page: this.page,
        pageSize: this.pageSize,
        search: this.search,
        categoriaIds: this.selectedCategoriaIds,
        stockMaximo: this.stockMaximo
      })
      .subscribe({
        next: (response) => {
          this.productos = response.items;
          this.totalItems = response.totalItems;
          this.totalPages = Math.max(response.totalPages || 1, 1);
          this.loading.set(false);
        },
        error: () => {
          this.error.set('No se pudo cargar la lista de productos');
          this.loading.set(false);
        }
      });
  }

  onSearch(): void {
    this.page = 1;
    this.loadProductos();
  }

  clearSearch(): void {
    this.search = '';
    this.page = 1;
    this.loadProductos();
  }

  openFilter(): void {
    this.filterCategoryEnabled = this.selectedCategoriaIds.length > 0;
    this.filterStockEnabled = this.stockMaximo !== null;
    this.filterCategoriaIdsDraft = [...this.selectedCategoriaIds];
    this.filterStockDraft = this.stockMaximo ?? 5;
    this.showFilterModal = true;
  }

  toggleCategoriaFilter(categoriaId: number, checked: boolean): void {
    if (checked) {
      this.filterCategoriaIdsDraft = [...new Set([...this.filterCategoriaIdsDraft, categoriaId])];
      return;
    }

    this.filterCategoriaIdsDraft = this.filterCategoriaIdsDraft.filter((id) => id !== categoriaId);
  }

  addFilterCategoria(event: Event): void {
    const selectEl = event.target as HTMLSelectElement;
    const id = Number(selectEl.value);
    if (id && !this.filterCategoriaIdsDraft.includes(id)) {
      this.filterCategoriaIdsDraft = [...this.filterCategoriaIdsDraft, id];
    }
    selectEl.value = '0';
  }

  removeFilterCategoria(categoriaId: number): void {
    this.filterCategoriaIdsDraft = this.filterCategoriaIdsDraft.filter((id) => id !== categoriaId);
  }

  get availableFilterCategorias(): Categoria[] {
    return this.categorias.filter((c) => !this.filterCategoriaIdsDraft.includes(c.categoriaId));
  }

  getCategoriaNombre(categoriaId: number): string {
    return this.categorias.find((c) => c.categoriaId === categoriaId)?.nombre ?? '';
  }

  applyFilter(): void {
    this.selectedCategoriaIds = this.filterCategoryEnabled ? [...this.filterCategoriaIdsDraft] : [];
    this.stockMaximo = this.filterStockEnabled ? this.filterStockDraft : null;
    this.page = 1;
    this.showFilterModal = false;
    this.loadProductos();
  }

  closeFilter(): void {
    this.showFilterModal = false;
  }

  openCreateModal(): void {
    this.editingProducto = null;
    this.selectedImage = null;

    this.form.reset({
      nombre: '',
      descripcion: '',
      precio: 1,
      stock: 0,
      categoriaId: this.categorias[0]?.categoriaId ?? 0
    });

    this.showProductoModal = true;
  }

  openEditModal(producto: Producto): void {
    this.editingProducto = producto;
    this.selectedImage = null;

    this.form.reset({
      nombre: producto.nombre,
      descripcion: producto.descripcion ?? '',
      precio: producto.precio,
      stock: producto.stock,
      categoriaId: producto.categoriaId
    });

    this.showProductoModal = true;
  }

  closeProductoModal(): void {
    this.showProductoModal = false;
    this.form.markAsPristine();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.item(0) ?? null;
    this.selectedImage = file;
  }

  saveProducto(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.actionLoading.set(true);
    this.feedback.set('');

    const raw = this.form.getRawValue();
    const payload: ProductoPayload = {
      nombre: raw.nombre,
      descripcion: raw.descripcion,
      precio: Number(raw.precio),
      stock: Number(raw.stock),
      categoriaId: Number(raw.categoriaId),
      imagen: this.selectedImage
    };

    const request$ = this.editingProducto
      ? this.productosService.update(this.editingProducto.productoId, payload)
      : this.productosService.create(payload);

    request$.subscribe({
      next: () => {
        this.actionLoading.set(false);
        this.showProductoModal = false;
        this.feedback.set(this.editingProducto ? 'Producto actualizado' : 'Producto creado');
        this.loadProductos();
      },
      error: (error) => {
        this.actionLoading.set(false);
        this.error.set(error?.error?.message ?? 'No se pudo guardar el producto');
      }
    });
  }

  deleteProducto(producto: Producto): void {
    const confirmed = confirm(`Eliminar producto: ${producto.nombre}?`);
    if (!confirmed) {
      return;
    }

    this.actionLoading.set(true);

    this.productosService.delete(producto.productoId).subscribe({
      next: () => {
        this.actionLoading.set(false);
        this.feedback.set('Producto eliminado');
        this.loadProductos();
      },
      error: (error) => {
        this.actionLoading.set(false);
        this.error.set(error?.error?.message ?? 'No se pudo eliminar el producto');
      }
    });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.page) {
      return;
    }

    this.page = page;
    this.loadProductos();
  }

  descargarReporte(): void {
    this.actionLoading.set(true);
    const umbral = this.stockMaximo ?? 5;

    this.reportesService.descargarPdfStockBajo(umbral).subscribe({
      next: (blob) => {
        this.actionLoading.set(false);

        const url = window.URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = `ReporteStockBajo_${new Date().toISOString().slice(0, 10)}.pdf`;
        anchor.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        this.actionLoading.set(false);
        this.error.set('No se pudo descargar el reporte PDF');
      }
    });
  }

  openEmailModal(): void {
    this.emailUmbral = this.stockMaximo ?? 5;
    this.showEmailModal = true;
  }

  closeEmailModal(): void {
    this.showEmailModal = false;
  }

  confirmEnviarEmail(): void {
    this.actionLoading.set(true);
    this.showEmailModal = false;

    this.reportesService.enviarPorEmail(this.emailUmbral).subscribe({
      next: () => {
        this.actionLoading.set(false);
        this.feedback.set('Reporte enviado por email exitosamente');
      },
      error: () => {
        this.actionLoading.set(false);
        this.error.set('No se pudo enviar el reporte por email');
      }
    });
  }

  trackProducto(_: number, producto: Producto): number {
    return producto.productoId;
  }

  private loadInitialData(): void {
    this.loading.set(true);
    forkJoin({
      categorias: this.categoriasService.getActivas(),
      productos: this.productosService.getPaginated({ page: this.page, pageSize: this.pageSize })
    }).subscribe({
      next: ({ categorias, productos }) => {
        this.categorias = categorias;
        this.productos = productos.items;
        this.totalItems = productos.totalItems;
        this.totalPages = Math.max(productos.totalPages || 1, 1);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('No se pudieron cargar categorias y productos');
        this.loading.set(false);
      }
    });
  }
}
