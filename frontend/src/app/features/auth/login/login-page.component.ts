import { CommonModule } from '@angular/common';
import { Component, inject, signal, OnDestroy } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login-page',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss'
})
export class LoginPageComponent implements OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private countdownInterval: ReturnType<typeof setInterval> | null = null;

  readonly showPassword = signal(false);
  readonly loading = signal(false);
  readonly errorMessage = signal('');
  readonly rateLimitCountdown = signal(0);

  readonly form = this.fb.nonNullable.group({
    correo: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
    rememberMe: [true]
  });

  submit(): void {
    if (this.form.invalid || this.rateLimitCountdown() > 0) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.errorMessage.set('');

    const { correo, password } = this.form.getRawValue();
    this.authService.login({ correo, password }).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        this.loading.set(false);

        if (error.status === 429) {
          this.startCooldown(60);
          return;
        }

        this.errorMessage.set(error?.error?.message ?? 'No se pudo iniciar sesion');
      }
    });
  }

  private startCooldown(seconds: number): void {
    this.errorMessage.set('');
    this.rateLimitCountdown.set(seconds);

    this.clearCountdown();
    this.countdownInterval = setInterval(() => {
      const remaining = this.rateLimitCountdown() - 1;
      this.rateLimitCountdown.set(remaining);
      if (remaining <= 0) {
        this.clearCountdown();
      }
    }, 1000);
  }

  private clearCountdown(): void {
    if (this.countdownInterval) {
      clearInterval(this.countdownInterval);
      this.countdownInterval = null;
    }
  }

  ngOnDestroy(): void {
    this.clearCountdown();
  }
}
