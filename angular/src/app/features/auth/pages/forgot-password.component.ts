import { ChangeDetectionStrategy, Component } from '@angular/core';
import { NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { AuthShellComponent } from '../../../shared/ui/auth-shell.component';
import { PremiumButtonComponent } from '../../../shared/ui/premium-button.component';
import { PremiumInputComponent } from '../../../shared/ui/premium-input.component';

interface ForgotPasswordForm {
  email: FormControl<string>;
}

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule, RouterLink, AuthShellComponent, PremiumButtonComponent, PremiumInputComponent],
  template: `
    <app-auth-shell eyebrow="RECUPERAR" headline="Recupera tu acceso" subtitle="Te enviaremos un enlace privado para continuar.">
      <form class="auth-form" [formGroup]="form" (ngSubmit)="submit()">
        <div class="auth-form__item" [style.animationDelay.ms]="380">
          <app-premium-input label="Email" type="email" autocomplete="email" [control]="form.controls.email" [errorMessage]="emailError" />
        </div>

        <div class="auth-form__item" [style.animationDelay.ms]="460">
          <app-premium-button label="Enviar recuperación" type="submit" [disabled]="form.invalid || submitted" />
        </div>

        <p class="auth-form__confirmation auth-form__item" [style.animationDelay.ms]="540" *ngIf="submitted">
          Revisa {{ form.controls.email.value }}. El enlace ya va en camino.
        </p>

        <a class="auth-form__link auth-form__item" [style.animationDelay.ms]="620" routerLink="/auth/login">
          Volver al login
        </a>
      </form>
    </app-auth-shell>
  `,
  styleUrl: './auth-pages.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ForgotPasswordComponent {
  protected submitted = false;

  protected readonly form = new FormGroup<ForgotPasswordForm>({
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] })
  });

  protected get emailError(): string {
    const control = this.form.controls.email;
    if (!control.touched && !control.dirty) {
      return '';
    }

    return control.hasError('email') ? 'Email inválido' : control.hasError('required') ? 'Email requerido' : '';
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitted = true;
  }
}
