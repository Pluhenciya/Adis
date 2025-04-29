import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthStateService } from '../../services/auth-state.service';

@Injectable({ providedIn: 'root' })
export class RoleGuard implements CanActivate {
  constructor(
    private authService: AuthStateService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const expectedRole = route.data['expectedRole'];
    const isAuthorized = this.authService.currentRole === expectedRole;

    if (!isAuthorized) {
      this.router.navigate(['/forbidden']);
      return false;
    }
    return true;
  }
}
