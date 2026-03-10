import { DOCUMENT } from '@angular/common';
import { inject, Injectable, signal } from '@angular/core';

export type ThemeMode = 'light' | 'dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly document = inject(DOCUMENT);
  private readonly key = 'gi_theme';

  readonly mode = signal<ThemeMode>(this.restoreTheme());

  constructor() {
    this.apply(this.mode());
  }

  toggle(): void {
    const next: ThemeMode = this.mode() === 'light' ? 'dark' : 'light';
    this.setMode(next);
  }

  setMode(mode: ThemeMode): void {
    this.mode.set(mode);
    localStorage.setItem(this.key, mode);
    this.apply(mode);
  }

  private restoreTheme(): ThemeMode {
    const stored = localStorage.getItem(this.key);
    return stored === 'dark' ? 'dark' : 'light';
  }

  private apply(mode: ThemeMode): void {
    this.document.body.classList.toggle('theme-dark', mode === 'dark');
  }
}
