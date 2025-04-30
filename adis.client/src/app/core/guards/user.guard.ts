import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthStateService } from '../../services/auth-state.service';
import { AuthService } from '../../services/auth.service';
import { RefreshTokenRequest } from '../../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authStateService: AuthStateService,
    private authService : AuthService,
    private router: Router
  ) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    if (this.authStateService.isAuthenticated()) {
      return true;
    }
    
    var tokens : RefreshTokenRequest = {
      accessToken : this.authStateService.accessToken!,
      refreshToken : this.authStateService.refreshToken!
    }

    this.authService.refreshToken(tokens).subscribe({
      next: (response) => {
        this.authStateService.login(response.accessToken, response.refreshToken);
        return true;
      },
      error: () => {
        // Перенаправляем на страницу логина с сохранением URL
        this.router.navigate(['/login'], {
          queryParams: { returnUrl: state.url }
        });
        return false;
      }
    });
    return false;
  }
}