import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthStateService } from '../../services/auth-state.service';
import { AuthService } from '../../services/auth.service';
import { RefreshTokenRequest } from '../../models/auth.model';
import { Observable, catchError, of, switchMap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authStateService: AuthStateService,
    private authService: AuthService,
    private router: Router
  ) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.authStateService.isAuthenticated().pipe(
      switchMap(isAuthenticated => {
        if (isAuthenticated) {
          return of(true);
        }
        
        // Попытка обновить токен
        const tokens: RefreshTokenRequest = {
          accessToken: this.authStateService.accessToken!,
          refreshToken: this.authStateService.refreshToken!
        };

        return this.authService.refreshToken(tokens).pipe(
          switchMap(response => {
            this.authStateService.login(response.accessToken, response.refreshToken);
            return of(true);
          }),
          catchError(() => {
            // Перенаправление при ошибке обновления
            this.router.navigate(['/login'], {
              queryParams: { returnUrl: state.url }
            });
            return of(false);
          })
        );
      }),
      catchError(() => {
        this.router.navigate(['/login']);
        return of(false);
      })
    );
  }
}