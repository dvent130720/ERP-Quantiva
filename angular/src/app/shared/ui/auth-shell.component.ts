import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { NgStyle } from '@angular/common';

@Component({
  selector: 'app-auth-shell',
  standalone: true,
  imports: [NgStyle],
  template: `
    <main class="auth-shell">
      <div class="auth-shell__orb auth-shell__orb--primary"></div>
      <div class="auth-shell__orb auth-shell__orb--halo"></div>
      <section class="auth-shell__content glass-panel">
        <p class="auth-shell__eyebrow" [style.animationDelay.ms]="120">{{ eyebrow }}</p>
        <h1 class="auth-shell__headline typewriter" [ngStyle]="{ '--type-delay': headlineDelay + 'ms' }">{{ headline }}</h1>
        <p class="auth-shell__subtitle" [style.animationDelay.ms]="240">{{ subtitle }}</p>
        <ng-content></ng-content>
      </section>
    </main>
  `,
  styleUrl: './auth-shell.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AuthShellComponent {
  @Input({ required: true }) eyebrow!: string;
  @Input({ required: true }) headline!: string;
  @Input({ required: true }) subtitle!: string;
  @Input() headlineDelay = 320;
}
