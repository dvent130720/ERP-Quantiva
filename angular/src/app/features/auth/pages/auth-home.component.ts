import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { AuthShellComponent } from '../../../shared/ui/auth-shell.component';
import { PremiumButtonComponent } from '../../../shared/ui/premium-button.component';
import { PremiumInputComponent } from '../../../shared/ui/premium-input.component';

interface LandingForm {
  email: FormControl<string>;
}

@Component({
  selector: 'app-auth-home',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, AuthShellComponent, PremiumButtonComponent, PremiumInputComponent],
  template: `
    <app-auth-shell eyebrow="VENTURACOM" headline="Impulsa" subtitle="Ingresa tu correo y despega con una experiencia fluida.">
      <form class="auth-form" [formGroup]="form" (ngSubmit)="continueWithEmail()">
        <div class="auth-form__item" [style.animationDelay.ms]="380">
          <app-premium-input
            label="Email"
            autocomplete="email"
            type="email"
            [control]="form.controls.email"
            [errorMessage]="emailError"
          />
        </div>

        <div class="auth-form__item" [style.animationDelay.ms]="460">
          <app-premium-button label="Comenzar" type="submit" [disabled]="form.invalid" />
        </div>

        <div class="auth-form__item" [style.animationDelay.ms]="540">
          <app-premium-button label="Continuar con Google" variant="secondary" [showGoogleMark]="true" />
        </div>

        <a class="auth-form__link auth-form__item" [style.animationDelay.ms]="620" routerLink="/auth/login">
          Ya tengo cuenta
        </a>
      </form>
    </app-auth-shell>
  `,
  styleUrl: './auth-pages.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AuthHomeComponent {
  private readonly router = inject(Router);

  protected readonly form = new FormGroup<LandingForm>({
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] })
  });

  protected get emailError(): string {
    const control = this.form.controls.email;
    if (!control.touched && !control.dirty) {
      return '';
    }

    if (control.hasError('required')) {
      return 'Tu email es obligatorio';
    }

    if (control.hasError('email')) {
      return 'Ingresa un email válido';
    }

    return '';
  }

  protected continueWithEmail(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    void this.router.navigate(['/auth/register'], {
      queryParams: { email: this.form.controls.email.value }
    });
  }
}
