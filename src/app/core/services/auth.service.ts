// core/services/auth.service.ts
import { Injectable, signal, inject } from '@angular/core';
import { Router } from '@angular/router';
import { delay, of } from 'rxjs';

export interface AuthUser {
  id: string;
  name: string;
  email: string;
  avatar?: string;
}

export interface LoginPayload     { email: string; password: string; }
export interface RegisterPayload  { name: string; email: string; password: string; }

@Injectable({ providedIn: 'root' })
export class AuthService {
  private router = inject(Router);

  // ── State ────────────────────────────────────────────────
  readonly currentUser = signal<AuthUser | null>(null);
  readonly isLoading   = signal(false);

  // ── Mock Methods ─────────────────────────────────────────
  login(payload: LoginPayload) {
    this.isLoading.set(true);
    return of({ success: true, user: { id: '1', name: 'Usuario', email: payload.email } })
      .pipe(delay(1200));
  }

  register(payload: RegisterPayload) {
    this.isLoading.set(true);
    return of({ success: true, user: { id: '1', name: payload.name, email: payload.email } })
      .pipe(delay(1500));
  }

  forgotPassword(email: string) {
    return of({ success: true }).pipe(delay(1200));
  }

  googleSignIn() {
    return of({ success: true }).pipe(delay(800));
  }

  logout(): void {
    this.currentUser.set(null);
    this.router.navigate(['/auth/landing']);
  }
}
