import { Routes } from '@angular/router';

export const appRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/auth/pages/auth-home.component').then((m) => m.AuthHomeComponent)
  },
  {
    path: 'auth/login',
    loadComponent: () => import('./features/auth/pages/login.component').then((m) => m.LoginComponent)
  },
  {
    path: 'auth/register',
    loadComponent: () => import('./features/auth/pages/register.component').then((m) => m.RegisterComponent)
  },
  {
    path: 'auth/forgot-password',
    loadComponent: () => import('./features/auth/pages/forgot-password.component').then((m) => m.ForgotPasswordComponent)
  },
  {
    path: '**',
    redirectTo: ''
  }
];
