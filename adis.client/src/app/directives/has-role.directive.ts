import { Directive, Input, OnDestroy, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AuthStateService } from '../services/auth-state.service';
import { Subscription } from 'rxjs';

@Directive({
    standalone: true,
    selector: '[appHasRole]'
})
export class HasRoleDirective implements OnDestroy {
  private sub!: Subscription;
  private requiredRoles: string[] = [];

  @Input() set appHasRole(roles: string | string[]) {
      // Принимаем как строку, так и массив
      this.requiredRoles = (Array.isArray(roles) ? roles : [roles])
                      .map(r => r?.toLowerCase() || '');
      this.updateView();
      
      // Подписываемся на изменения роли
      this.sub = this.authService.role$.subscribe(() => this.updateView());
  }

  constructor(
      private templateRef: TemplateRef<any>,
      private viewContainer: ViewContainerRef,
      private authService: AuthStateService
  ) {}

  private updateView(): void {
      const hasAccess = this.authService.currentRole !== null ? this.requiredRoles.includes(this.authService.currentRole.toLowerCase()) : false;
      this.viewContainer.clear();
      if (hasAccess) {
          this.viewContainer.createEmbeddedView(this.templateRef);
      }
  }

  ngOnDestroy(): void {
      if (this.sub) {
          this.sub.unsubscribe();
      }
  }
}