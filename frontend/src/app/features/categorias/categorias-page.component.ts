import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Categoria } from '../../core/models/categoria.models';
import { AuthService } from '../../core/services/auth.service';
import { CategoriasService } from '../../core/services/categorias.service';

@Component({
  selector: 'app-categorias-page',
  imports: [CommonModule, FormsModule],
  templateUrl: './categorias-page.component.html',
  styleUrl: './categorias-page.component.scss'
})
export class CategoriasPageComponent implements OnInit {
  private readonly categoriasService = inject(CategoriasService);
  private readonly authService = inject(AuthService);

  readonly loading = signal(true);
  readonly actionLoading = signal(false);
  readonly error = signal('');
  readonly feedback = signal('');

  categorias: Categoria[] = [];
  search = '';

  showModal = false;
  editingCategoria: Categoria | null = null;
  modalNombre = '';

  get isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  get filteredCategorias(): Categoria[] {
    if (!this.search.trim()) return this.categorias;
    const query = this.search.toLowerCase().trim();
    return this.categorias.filter((c) => c.nombre.toLowerCase().includes(query));
  }

  ngOnInit(): void {
    this.loadCategorias();
  }

  loadCategorias(): void {
    this.loading.set(true);
    this.categoriasService.getActivas().subscribe({
      next: (data) => {
        this.categorias = data;
        this.loading.set(false);
      },
      error: () => {
        this.error.set('No se pudieron cargar las categorías');
        this.loading.set(false);
      }
    });
  }

  openCreateModal(): void {
    this.editingCategoria = null;
    this.modalNombre = '';
    this.showModal = true;
  }

  openEditModal(categoria: Categoria): void {
    this.editingCategoria = categoria;
    this.modalNombre = categoria.nombre;
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.modalNombre = '';
    this.editingCategoria = null;
  }

  saveCategoria(): void {
    const nombre = this.modalNombre.trim();
    if (!nombre || nombre.length < 3) return;

    this.actionLoading.set(true);
    this.feedback.set('');
    this.error.set('');

    const request$ = this.editingCategoria
      ? this.categoriasService.actualizar(this.editingCategoria.categoriaId, nombre)
      : this.categoriasService.crear(nombre);

    request$.subscribe({
      next: () => {
        this.actionLoading.set(false);
        this.showModal = false;
        this.modalNombre = '';
        this.feedback.set(this.editingCategoria ? 'Categoría actualizada' : 'Categoría creada');
        this.editingCategoria = null;
        this.loadCategorias();
      },
      error: (err) => {
        this.actionLoading.set(false);
        this.error.set(err?.error?.message ?? 'No se pudo guardar la categoría');
      }
    });
  }

  deleteCategoria(categoria: Categoria): void {
    const confirmed = confirm(`¿Eliminar categoría "${categoria.nombre}"?`);
    if (!confirmed) return;

    this.actionLoading.set(true);
    this.feedback.set('');
    this.error.set('');

    this.categoriasService.eliminar(categoria.categoriaId).subscribe({
      next: () => {
        this.actionLoading.set(false);
        this.feedback.set('Categoría eliminada');
        this.loadCategorias();
      },
      error: (err) => {
        this.actionLoading.set(false);
        this.error.set(err?.error?.message ?? 'No se pudo eliminar la categoría');
      }
    });
  }
}
