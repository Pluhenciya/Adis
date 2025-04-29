import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { AuthStateService } from '../services/auth-state.service';

@Directive({
    standalone: false,
    selector: '[appHasRole]'
})
export class HasRoleDirective {
  private hasView = false;

  @Input() set appHasRole(role: string) {
    const hasAccess = this.authService.currentRole === role;
    
    if (hasAccess && !this.hasView) {
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.hasView = true;
    } else if (!hasAccess && this.hasView) {
      this.viewContainer.clear();
      this.hasView = false;
    }
  }

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private authService: AuthStateService
  ) { }
}