// features/auth/register/register.component.ts
import {
  Component,
  OnInit,
  ChangeDetectionStrategy,
  signal,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
} from '@angular/forms';
import { AuthLayoutComponent } from '../../../shared/ui/auth-layout/auth-layout.component';
import { VcButtonComponent }   from '../../../shared/ui/button/vc-button.component';
import { VcInputComponent }     from '../../../shared/ui/input/vc-input.component';
import { AuthService }          from '../../../core/services/auth.service';

@Component({
  selector: 'vc-register',
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
  templateUrl: './register.component.html',
  styleUrls:   ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
  private fb    = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private auth  = inject(AuthService);

  form!: FormGroup;
  isLoading = signal(false);

  ngOnInit(): void {
    const email = this.route.snapshot.queryParamMap.get('email') ?? '';
    this.form = this.fb.group({
      name:     ['', [Validators.required, Validators.minLength(2)]],
      email:    [email, [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
    });
  }

  getError(field: string): string {
    const ctrl: AbstractControl | null = this.form.get(field);
    if (!ctrl?.touched) return '';
    if (ctrl.hasError('required'))   return 'Campo requerido';
    if (ctrl.hasError('email'))      return 'Correo no válido';
    if (ctrl.hasError('minlength'))  {
      const min = ctrl.errors?.['minlength']?.requiredLength;
      return field === 'password'
        ? `Mínimo ${min} caracteres`
        : 'Nombre demasiado corto';
    }
    return '';
  }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.isLoading.set(true);
    this.auth.register(this.form.value).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/dashboard']);
      },
      error: () => this.isLoading.set(false),
    });
  }
}
