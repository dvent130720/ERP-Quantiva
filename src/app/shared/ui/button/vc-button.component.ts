// shared/ui/button/vc-button.component.ts
import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

export type ButtonVariant = 'primary' | 'ghost' | 'google' | 'link';
export type ButtonSize    = 'sm' | 'md' | 'lg';

@Component({
  selector: 'vc-button',
  standalone: true,
  imports: [CommonModule, MatProgressSpinnerModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <button
      [type]="type"
      [disabled]="disabled || loading"
      [class]="'vc-btn vc-btn--' + variant + ' vc-btn--' + size"
      [class.vc-btn--loading]="loading"
      [class.vc-btn--full]="fullWidth"
      (click)="clicked.emit($event)">

      <!-- Google icon -->
      <span *ngIf="variant === 'google'" class="vc-btn__icon" aria-hidden="true">
        <svg width="18" height="18" viewBox="0 0 18 18">
          <path fill="#4285F4" d="M17.64 9.2c0-.637-.057-1.251-.164-1.84H9v3.481h4.844c-.209 1.125-.843 2.078-1.796 2.717v2.258h2.908c1.702-1.567 2.684-3.875 2.684-6.615z"/>
          <path fill="#34A853" d="M9 18c2.43 0 4.467-.806 5.956-2.18l-2.908-2.259c-.806.54-1.837.86-3.048.86-2.344 0-4.328-1.584-5.036-3.711H.957v2.332A8.997 8.997 0 0 0 9 18z"/>
          <path fill="#FBBC05" d="M3.964 10.71A5.41 5.41 0 0 1 3.682 9c0-.593.102-1.17.282-1.71V4.958H.957A8.996 8.996 0 0 0 0 9c0 1.452.348 2.827.957 4.042l3.007-2.332z"/>
          <path fill="#EA4335" d="M9 3.58c1.321 0 2.508.454 3.44 1.345l2.582-2.58C13.463.891 11.426 0 9 0A8.997 8.997 0 0 0 .957 4.958L3.964 7.29C4.672 5.163 6.656 3.58 9 3.58z"/>
        </svg>
      </span>

      <!-- Loading spinner -->
      <mat-spinner *ngIf="loading" diameter="18" class="vc-btn__spinner"></mat-spinner>

      <!-- Label -->
      <span class="vc-btn__label" [class.sr-only]="loading && hideTextOnLoad">
        {{ label }}
      </span>

    </button>
  `,
  styleUrls: ['./vc-button.component.scss'],
})
export class VcButtonComponent {
  @Input() label      = '';
  @Input() variant: ButtonVariant = 'primary';
  @Input() size: ButtonSize       = 'md';
  @Input() type: 'button' | 'submit' | 'reset' = 'button';
  @Input() disabled   = false;
  @Input() loading    = false;
  @Input() fullWidth  = false;
  @Input() hideTextOnLoad = false;

  @Output() clicked = new EventEmitter<MouseEvent>();
}
