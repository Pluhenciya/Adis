import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { BehaviorSubject, Observable, of, switchMap, tap } from 'rxjs';
import { AuthService } from './auth.service';
import { RefreshTokenRequest } from '../models/auth.model';

interface DecodedToken {
  roles: string;
  exp: number;
  sub: string;
  email: string;
}

@Injectable({ providedIn: 'root' })
export class AuthStateService {
  private roleSubject = new BehaviorSubject<string | null>(null);
  private emailSubject = new BehaviorSubject<string | null>(null);
  private userIdSubject = new BehaviorSubject<string | null>(null);
  private isRefreshing = false;
  
  public role$ = this.roleSubject.asObservable();
  public email$ = this.emailSubject.asObservable();

  constructor(
    private router: Router,
    private authService: AuthService
  ) {
    this.initializeAuthState();
  }

  private initializeAuthState(): void {
    const token = this.accessToken;
    if (token) {
      try {
        const decoded = jwtDecode<DecodedToken>(token);
        this.updateAuthState(decoded);
      } catch {
        this.clearAuthData();
      }
    }
  }

  get accessToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  get refreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }

  get currentRole(): string | null {
    return this.roleSubject.value;
  }

  get currentEmail(): string | null {
    return this.emailSubject.value;
  }

  get currentUserId(): string | null {
    return this.userIdSubject.value;
  }

  isAuthenticated(): Observable<boolean> {
    if (!this.accessToken) return of(false);

    if (this.isTokenExpired()) {
      if (this.isRefreshing) {
        return this.role$.pipe(switchMap(() => of(!!this.accessToken)));
      }
      return this.handleTokenRefresh();
    }
    
    return of(true);
  }

  private handleTokenRefresh(): Observable<boolean> {
    if (!this.refreshToken) {
      this.clearAuthData();
      return of(false);
    }

    this.isRefreshing = true;
    const tokens: RefreshTokenRequest = {
      accessToken: this.accessToken!,
      refreshToken: this.refreshToken!
    };

    return this.authService.refreshToken(tokens).pipe(
      tap({
        next: (response) => {
          this.login(response.accessToken, response.refreshToken);
          this.isRefreshing = false;
        },
        error: () => {
          this.clearAuthData();
          this.isRefreshing = false;
        }
      }),
      switchMap(() => of(!!this.accessToken))
    );
  }

  isAdmin(): boolean {
    return this.currentRole === 'Admin';
  }

  private isTokenExpired(): boolean {
    const token = this.accessToken;
    if (!token) return true;
    try {
      const decoded = jwtDecode<DecodedToken>(token);
      return decoded.exp * 1000 < Date.now();
    } catch {
      return true;
    }
  }

  login(accessToken: string, refreshToken: string): void {
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
    const decoded = jwtDecode<DecodedToken>(accessToken);
    this.updateAuthState(decoded);
  }

  logout(): void {
    this.clearAuthData();
    this.router.navigate(['/login']);
  }

  private updateAuthState(decoded: DecodedToken): void {
    this.roleSubject.next(decoded.roles.toString());
    this.emailSubject.next(decoded.email);
    this.userIdSubject.next(decoded.sub);
  }

  private clearAuthData(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    this.roleSubject.next(null);
    this.emailSubject.next(null);
    this.userIdSubject.next(null);
  }
}