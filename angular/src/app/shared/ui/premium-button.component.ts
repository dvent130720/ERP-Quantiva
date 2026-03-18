import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { NgClass, NgIf } from '@angular/common';

@Component({
  selector: 'app-premium-button',
  standalone: true,
  imports: [NgClass, NgIf],
  template: `
    <button
      class="premium-button"
      [ngClass]="variant"
      [type]="type"
      [disabled]="disabled"
    >
      <span class="premium-button__icon" *ngIf="showGoogleMark" aria-hidden="true">G</span>
      <span>{{ label }}</span>
    </button>
  `,
  styleUrl: './premium-button.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PremiumButtonComponent {
  @Input({ required: true }) label!: string;
  @Input() type: 'button' | 'submit' = 'button';
  @Input() variant: 'primary' | 'secondary' = 'primary';
  @Input() disabled = false;
  @Input() showGoogleMark = false;
}
