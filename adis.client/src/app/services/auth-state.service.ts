import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { BehaviorSubject } from 'rxjs';

interface DecodedToken {
  roles: string;  
  exp: number;
  sub: string;
  email: string;
}

@Injectable({ providedIn: 'root' })
export class AuthStateService {
  private roleSubject = new BehaviorSubject<string | null>(null);
  public role$ = this.roleSubject.asObservable();

  constructor(private router: Router) {
    this.initializeAuthState();
  }

  private initializeAuthState(): void {
    const token = this.accessToken;
    if (token) {
      try {
        const decoded = jwtDecode<DecodedToken>(token);
        this.roleSubject.next(decoded.roles.toString());
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

  isAuthenticated(): boolean {
    return !!this.accessToken && !this.isTokenExpired();
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
    console.log(decoded.roles)
    this.roleSubject.next(decoded.roles.toString());
  }

  logout(): void {
    this.clearAuthData();
    this.router.navigate(['/login']);
  }

  private clearAuthData(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    this.roleSubject.next(null);
  }
}