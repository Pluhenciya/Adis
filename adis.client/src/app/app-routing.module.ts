import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProjectListPageComponent } from './pages/project-list-page/project-list-page.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { UserListPageComponent } from './pages/user-list-page/user-list-page.component';
import { RoleGuard } from './core/guards/role.guard';
import { AuthGuard } from './core/guards/user.guard';
import { ForbiddenPageComponent } from './pages/forbidden-page/forbidden-page.component';
import { ProjectDetailsPageComponent } from './pages/project-details-page/project-details-page.component';
import { TaskBoardPageComponent } from './pages/task-board-page/task-board-page.component';
import { RoleBasedRedirectGuard } from './core/guards/role-based-redirect.guard';
import { DocumentListPageComponent } from './pages/document-list-page/document-list-page.component';
import { NeuroGuidePageComponent } from './pages/neuro-guide-page/neuro-guide-page.component';

const routes: Routes = [
  { 
    path: 'login', 
    component: LoginPageComponent, 
    title: 'Вход в систему' 
  },
  {
    path: 'admin',
    children: [
      {
        path: 'users',
        component: UserListPageComponent,
        canActivate: [AuthGuard, RoleGuard],
        data: { expectedRole: 'Admin' },
        title: 'Пользователи'
      }
    ]
  },
  { 
    path: 'forbidden', 
    component: ForbiddenPageComponent,
    title: 'Ошибка 403'
  },
  {
    path: 'projects',
    children: [
      {
        path: '',
        component: ProjectListPageComponent,
        title: 'Проекты'
      },
      { 
        path: ':id', 
        component: ProjectDetailsPageComponent,
        title: 'Проект'
      }
    ]
  },
  { 
    path: 'tasks', 
    component: TaskBoardPageComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { expectedRole: 'Projecter' },
    title: 'Задачи'
  },
  {
    path: '',
    canActivate: [RoleBasedRedirectGuard],
    children: [] 
  },
  { 
    path: 'documents', 
    component: DocumentListPageComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { expectedRole: 'Admin' },
    title: 'База документов'
  },
  { 
    path: 'neuro-guide', 
    component: NeuroGuidePageComponent,
    canActivate: [AuthGuard],
    title: 'Нейросправочник'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
