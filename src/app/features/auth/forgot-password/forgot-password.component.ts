// features/auth/forgot-password/forgot-password.component.ts
import {
  Component,
  OnInit,
  ChangeDetectionStrategy,
  signal,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
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
  selector: 'vc-forgot-password',
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
  templateUrl: './forgot-password.component.html',
  styleUrls:   ['./forgot-password.component.scss'],
})
export class ForgotPasswordComponent implements OnInit {
  private fb   = inject(FormBuilder);
  private auth = inject(AuthService);

  form!: FormGroup;
  isLoading = signal(false);
  sent      = signal(false);
  sentEmail = signal('');

  ngOnInit(): void {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
    });
  }

  getEmailError(): string {
    const ctrl = this.form.get('email');
    if (!ctrl?.touched) return '';
    if (ctrl.hasError('required')) return 'Ingresa tu correo';
    if (ctrl.hasError('email'))    return 'Correo no válido';
    return '';
  }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.isLoading.set(true);
    const email = this.form.value.email;
    this.auth.forgotPassword(email).subscribe(() => {
      this.isLoading.set(false);
      this.sentEmail.set(email);
      this.sent.set(true);
    });
  }
}
