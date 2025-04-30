import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { AuthStateService } from '../services/auth-state.service';
import { Subscription } from 'rxjs';

@Directive({
    standalone: false,
    selector: '[appHasRole]'
})
export class HasRoleDirective {
  private sub!: Subscription;
  private requiredRole!: string;

  @Input() set appHasRole(role: string) {
    this.requiredRole = role;
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
    const hasAccess = this.authService.currentRole === this.requiredRole;
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