import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { AuthShellComponent } from '../../../shared/ui/auth-shell.component';
import { PremiumButtonComponent } from '../../../shared/ui/premium-button.component';
import { PremiumInputComponent } from '../../../shared/ui/premium-input.component';

interface RegisterForm {
  name: FormControl<string>;
  email: FormControl<string>;
  password: FormControl<string>;
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, AuthShellComponent, PremiumButtonComponent, PremiumInputComponent],
  template: `
    <app-auth-shell eyebrow="REGISTRO" headline="Activa tu cuenta" subtitle="Abre tu cuenta y centraliza tu operación desde hoy.">
      <form class="auth-form" [formGroup]="form" (ngSubmit)="submit()">
        <div class="auth-form__item" [style.animationDelay.ms]="360">
          <app-premium-input label="Nombre" autocomplete="name" [control]="form.controls.name" [errorMessage]="nameError" />
        </div>

        <div class="auth-form__item" [style.animationDelay.ms]="440">
          <app-premium-input label="Email" type="email" autocomplete="email" [control]="form.controls.email" [errorMessage]="emailError" />
        </div>

        <div class="auth-form__item" [style.animationDelay.ms]="520">
          <app-premium-input label="Password" type="password" autocomplete="new-password" [control]="form.controls.password" [errorMessage]="passwordError" hint="Usa 8+ caracteres" />
        </div>

        <div class="auth-form__item" [style.animationDelay.ms]="600">
          <app-premium-button label="Crear cuenta" type="submit" [disabled]="form.invalid" />
        </div>

        <a class="auth-form__link auth-form__item" [style.animationDelay.ms]="680" routerLink="/auth/login">
          Ya tengo cuenta
        </a>
      </form>
    </app-auth-shell>
  `,
  styleUrl: './auth-pages.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegisterComponent {
  private readonly route = inject(ActivatedRoute);

  protected readonly form = new FormGroup<RegisterForm>({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(2)] }),
    email: new FormControl(this.route.snapshot.queryParamMap.get('email') ?? '', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(8)] })
  });

  protected get nameError(): string {
    const control = this.form.controls.name;
    if (!control.touched && !control.dirty) {
      return '';
    }

    return control.hasError('minlength') ? 'Mínimo 2 caracteres' : control.hasError('required') ? 'Nombre requerido' : '';
  }

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
