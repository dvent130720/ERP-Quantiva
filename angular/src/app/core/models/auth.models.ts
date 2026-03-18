export type AuthMode = 'landing' | 'login' | 'register' | 'forgot-password';

export interface AuthShellContent {
  readonly mode: AuthMode;
  readonly eyebrow: string;
  readonly headline: string;
  readonly subtitle: string;
}

export interface RecoveryState {
  readonly submitted: boolean;
  readonly email: string;
}
