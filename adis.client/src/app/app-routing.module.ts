import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProjectListPageComponent } from './pages/project-list-page/project-list-page.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { UserListPageComponent } from './pages/user-list-page/user-list-page.component';
import { RoleGuard } from './core/guards/role.guard';
import { AuthGuard } from './core/guards/user.guard';

const routes: Routes = [
  { path: 'login', component: LoginPageComponent },
  {
    path: 'admin',
    children: [
      {
        path: 'users',
        component: UserListPageComponent,
        canActivate: [AuthGuard, RoleGuard],
        data: { expectedRole: 'Admin' }
      }
    ]
  },
  // { path: 'forbidden', component: ForbiddenComponent },
  {
    path: '',
    component: ProjectListPageComponent,
    title: 'Главная',
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
