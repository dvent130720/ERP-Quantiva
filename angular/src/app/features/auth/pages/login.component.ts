import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { AuthShellComponent } from '../../../shared/ui/auth-shell.component';
import { PremiumButtonComponent } from '../../../shared/ui/premium-button.component';
import { PremiumInputComponent } from '../../../shared/ui/premium-input.component';

interface LoginForm {
  email: FormControl<string>;
  password: FormControl<string>;
}

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, AuthShellComponent, PremiumButtonComponent, PremiumInputComponent],
  template: `
    <app-auth-shell eyebrow="ACCESO" headline="Vuelve a entrar" subtitle="Entra con claridad, sin distracciones ni pasos extra.">
      <form class="auth-form" [formGroup]="form" (ngSubmit)="submit()">
        <div class="auth-form__item" [style.animationDelay.ms]="360">
          <app-premium-input label="Email" type="email" autocomplete="email" [control]="form.controls.email" [errorMessage]="emailError" />
        </div>

        <div class="auth-form__item" [style.animationDelay.ms]="440">
          <app-premium-input label="Password" type="password" autocomplete="current-password" [control]="form.controls.password" [errorMessage]="passwordError" />
        </div>

        <div class="auth-form__item auth-form__split" [style.animationDelay.ms]="520">
          <app-premium-button label="Entrar" type="submit" [disabled]="form.invalid" />
          <a class="auth-form__micro-link" routerLink="/auth/forgot-password">Olvidé mi contraseña</a>
        </div>

        <a class="auth-form__link auth-form__item" [style.animationDelay.ms]="600" routerLink="/auth/register">
          Crear una cuenta
        </a>
      </form>
    </app-auth-shell>
  `,
  styleUrl: './auth-pages.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoginComponent {
  protected readonly form = new FormGroup<LoginForm>({
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(8)] })
  });

  protected get emailError(): string {
    const control = this.form.controls.email;
    if (!control.touched && !control.dirty) {
      return '';
    }

    return control.hasError('email') ? 'Email inválido' : control.hasError('required') ? 'Email requerido' : '';
  }

  protected get passwordError(): string {
    const control = this.form.controls.password;
    if (!control.touched && !control.dirty) {
      return '';
    }

    return control.hasError('minlength') ? 'Mínimo 8 caracteres' : control.hasError('required') ? 'Password requerida' : '';
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
    }
  }
}
