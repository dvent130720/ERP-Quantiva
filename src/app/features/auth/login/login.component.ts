// features/auth/login/login.component.ts
import {
  Component,
  OnInit,
  ChangeDetectionStrategy,
  signal,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { AuthLayoutComponent } from '../../../shared/ui/auth-layout/auth-layout.component';
import { VcButtonComponent }   from '../../../shared/ui/button/vc-button.component';
import { VcInputComponent }     from '../../../shared/ui/input/vc-input.component';
import { AuthService }          from '../../../core/services/auth.service';

@Component({
  selector: 'vc-login',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    AuthLayoutComponent,
    VcButtonComponent,
    VcInputComponent,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './login.component.html',
  styleUrls:   ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  private fb     = inject(FormBuilder);
  private router = inject(Router);
  private auth   = inject(AuthService);

  form!: FormGroup;
  isLoading = signal(false);
  apiError  = signal('');

  ngOnInit(): void {
    this.form = this.fb.group({
      email:    ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
    });
  }

  getError(field: string): string {
    const ctrl = this.form.get(field);
    if (!ctrl?.touched) return '';
    if (ctrl.hasError('required')) return field === 'email' ? 'Ingresa tu correo' : 'Ingresa tu contraseña';
    if (ctrl.hasError('email'))    return 'Correo no válido';
    return '';
  }

  onSubmit(): void {
    this.apiError.set('');
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.isLoading.set(true);
    this.auth.login(this.form.value).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/dashboard']);
      },
      error: () => {
        this.isLoading.set(false);
        this.apiError.set('Credenciales incorrectas. Intenta de nuevo.');
      },
    });
  }

  onGoogleSignIn(): void {
    this.auth.googleSignIn().subscribe(() => this.router.navigate(['/dashboard']));
  }
}
