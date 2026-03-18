import { animate, style, transition, trigger } from '@angular/animations';

export const fadeUpAnimation = trigger('fadeUp', [
  transition(':enter', [
    style({ opacity: 0, transform: 'translateY(28px)' }),
    animate('700ms cubic-bezier(0.22, 1, 0.36, 1)', style({ opacity: 1, transform: 'translateY(0)' }))
  ])
]);
