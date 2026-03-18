// app/app.routes.ts
import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'auth',
    children: [
      {
        path: 'landing',
        loadComponent: () =>
          import('./features/auth/landing/landing.component').then(
            m => m.LandingComponent
          ),
      },
      {
        path: 'login',
        loadComponent: () =>
          import('./features/auth/login/login.component').then(
            m => m.LoginComponent
          ),
      },
      {
        path: 'register',
        loadComponent: () =>
          import('./features/auth/register/register.component').then(
            m => m.RegisterComponent
          ),
      },
      {
        path: 'forgot-password',
        loadComponent: () =>
          import('./features/auth/forgot-password/forgot-password.component').then(
            m => m.ForgotPasswordComponent
          ),
      },
      { path: '', redirectTo: 'landing', pathMatch: 'full' },
    ],
  },
  { path: '', redirectTo: '/auth/landing', pathMatch: 'full' },
  { path: '**',  redirectTo: '/auth/landing' },
];
