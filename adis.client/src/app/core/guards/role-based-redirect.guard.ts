import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthStateService } from '../../services/auth-state.service';

@Injectable({
  providedIn: 'root'
})
export class RoleBasedRedirectGuard implements CanActivate {
  constructor(
    private authService: AuthStateService,
    private router: Router
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    var role = this.authService.currentRole;
    if(!role)
        return this.router.parseUrl('/projects');

    switch(role) {
        case 'Projecter':
            return this.router.parseUrl('/tasks');
        default:
            return this.router.parseUrl('/projects');
    }
  }
}