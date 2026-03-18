// shared/ui/input/vc-input.component.ts
import {
  Component,
  Input,
  forwardRef,
  ChangeDetectionStrategy,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ControlValueAccessor,
  NG_VALUE_ACCESSOR,
  ReactiveFormsModule,
} from '@angular/forms';

export type InputType = 'text' | 'email' | 'password' | 'tel';

@Component({
  selector: 'vc-input',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => VcInputComponent),
      multi: true,
    },
  ],
  template: `
    <div class="vc-input" [class.vc-input--error]="error" [class.vc-input--focused]="focused()">
      <label *ngIf="label" [for]="inputId" class="vc-input__label">{{ label }}</label>

      <div class="vc-input__wrapper">
        <input
          [id]="inputId"
          [type]="showPassword() ? 'text' : type"
          [placeholder]="placeholder"
          [disabled]="isDisabled()"
          [value]="value()"
          class="vc-input__field"
          autocomplete="off"
          spellcheck="false"
          (input)="onInput($event)"
          (blur)="onBlur()"
          (focus)="onFocus()"
        />

        <!-- Toggle password visibility -->
        <button
          *ngIf="type === 'password'"
          type="button"
          class="vc-input__toggle"
          [attr.aria-label]="showPassword() ? 'Ocultar contraseña' : 'Mostrar contraseña'"
          (click)="togglePassword()">
          <svg *ngIf="!showPassword()" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
            <path d="M15 12a3 3 0 1 1-6 0 3 3 0 0 1 6 0z"/>
            <path d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"/>
          </svg>
          <svg *ngIf="showPassword()" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
            <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/>
            <line x1="1" y1="1" x2="23" y2="23"/>
          </svg>
        </button>
      </div>

      <!-- Error message -->
      <span *ngIf="error" class="vc-input__error" role="alert">
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/>
        </svg>
        {{ error }}
      </span>
    </div>
  `,
  styleUrls: ['./vc-input.component.scss'],
})
export class VcInputComponent implements ControlValueAccessor {
  @Input() label      = '';
  @Input() placeholder = '';
  @Input() type: InputType = 'text';
  @Input() error      = '';
  @Input() inputId    = `vc-input-${Math.random().toString(36).slice(2, 7)}`;

  // Signals
  value     = signal('');
  focused   = signal(false);
  isDisabled = signal(false);
  showPassword = signal(false);

  private onChange  = (_: string) => {};
  private onTouched = () => {};

  onInput(event: Event): void {
    const val = (event.target as HTMLInputElement).value;
    this.value.set(val);
    this.onChange(val);
  }

  onFocus(): void { this.focused.set(true); }

  onBlur(): void {
    this.focused.set(false);
    this.onTouched();
  }

  togglePassword(): void {
    this.showPassword.update(v => !v);
  }

  // CVA
  writeValue(val: string): void     { this.value.set(val ?? ''); }
  registerOnChange(fn: (_: string) => void): void { this.onChange = fn; }
  registerOnTouched(fn: () => void): void         { this.onTouched = fn; }
  setDisabledState(isDisabled: boolean): void      { this.isDisabled.set(isDisabled); }
}
