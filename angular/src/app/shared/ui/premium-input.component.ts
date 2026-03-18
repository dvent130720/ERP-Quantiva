import { NgIf } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { AbstractControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-premium-input',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule, MatFormFieldModule, MatInputModule],
  template: `
    <mat-form-field class="premium-field" appearance="outline" subscriptSizing="dynamic">
      <mat-label>{{ label }}</mat-label>
      <input matInput [type]="type" [formControl]="control" [autocomplete]="autocomplete" />
      <mat-hint *ngIf="hint && !errorMessage">{{ hint }}</mat-hint>
      <mat-error *ngIf="errorMessage">{{ errorMessage }}</mat-error>
    </mat-form-field>
  `,
  styleUrl: './premium-input.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PremiumInputComponent {
  @Input({ required: true }) label!: string;
  @Input({ required: true }) control!: AbstractControl<string>;
  @Input() type: 'text' | 'email' | 'password' = 'text';
  @Input() autocomplete = 'off';
  @Input() hint = '';
  @Input() errorMessage = '';
}
