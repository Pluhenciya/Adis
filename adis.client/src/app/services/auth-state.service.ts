import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { BehaviorSubject, Observable, catchError, delay, filter, map, of, shareReplay, switchMap, take, tap, throwError, timeout } from 'rxjs';
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
  private refreshTokenRequest: Observable<boolean> | null = null;
  private readonly REFRESH_TIMEOUT = 5000; // 5 seconds timeout
  
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
      if (this.refreshTokenRequest) {
        return this.refreshTokenRequest;
      }
      return this.handleTokenRefresh();
    }
    
    return of(true);
  }

  private handleTokenRefresh(): Observable<boolean> {
    if (!this.refreshToken || !this.accessToken) {
      this.clearAuthData();
      return of(false);
    }

    const request: RefreshTokenRequest = {
      refreshToken: this.refreshToken,
      accessToken: this.accessToken
    };

    this.refreshTokenRequest = this.authService.refreshToken(request).pipe(
      timeout(this.REFRESH_TIMEOUT),
      tap({
        next: (response) => {
          this.login(response.accessToken, response.refreshToken);
          this.refreshTokenRequest = null;
        },
        error: (err) => {
          console.error('Refresh token failed:', err);
          this.refreshTokenRequest = null;
          if (err.name !== 'TimeoutError') {
            this.clearAuthData();
          }
        }
      }),
      map(() => true),
      catchError((err) => {
        if (err.name === 'TimeoutError') {
          console.error('Refresh token timed out');
          return of(false).pipe(delay(1000)); 
        }
        return throwError(() => err);
      }),
      shareReplay(1) 
    );

    return this.refreshTokenRequest;
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