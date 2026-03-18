// features/auth/landing/landing.component.ts
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
  selector: 'vc-landing',
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
  templateUrl: './landing.component.html',
  styleUrls:   ['./landing.component.scss'],
})
export class LandingComponent implements OnInit {
  private fb     = inject(FormBuilder);
  private router = inject(Router);
  private auth   = inject(AuthService);

  form!: FormGroup;
  isLoading = signal(false);
  typewriterDone = signal(false);

  readonly headline = 'Trabajo en equipo, sin fricción.';
  displayedHeadline = signal('');

  ngOnInit(): void {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
    });
    this.startTypewriter();
  }

  private startTypewriter(): void {
    const chars = this.headline.split('');
    let i = 0;
    const tick = () => {
      if (i < chars.length) {
        this.displayedHeadline.update(v => v + chars[i]);
        i++;
        setTimeout(tick, 48);
      } else {
        this.typewriterDone.set(true);
      }
    };
    setTimeout(tick, 600);
  }

  getEmailError(): string {
    const ctrl = this.form.get('email');
    if (ctrl?.touched) {
      if (ctrl.hasError('required')) return 'Ingresa tu correo';
      if (ctrl.hasError('email'))    return 'Correo no válido';
    }
    return '';
  }

  onContinue(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    const email = this.form.value.email;
    this.router.navigate(['/auth/register'], { queryParams: { email } });
  }

  onGoogleSignIn(): void {
    this.isLoading.set(true);
    this.auth.googleSignIn().subscribe(() => {
      this.isLoading.set(false);
      this.router.navigate(['/dashboard']);
    });
  }
}
