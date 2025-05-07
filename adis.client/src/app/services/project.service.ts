import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GetProjectDto } from '../models/project.model';
import { environment } from '../environments/environment';

interface ProjectsResponse {
  projects: GetProjectDto[];
  totalCount: number;
  page: number;
  pageSize: number;
}


@Injectable({
  providedIn: 'root'
})

export class ProjectService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getProjects(requestParams: {
    page: number;
    pageSize: number;
    sortField?: string;
    sortOrder?: 'asc' | 'desc';
    status?: string;
    targetDate?: string; 
    startDateFrom?: Date;
    startDateTo?: Date;
  }): Observable<ProjectsResponse> {
    let httpParams = new HttpParams()
      .set('page', requestParams.page.toString())
      .set('pageSize', requestParams.pageSize.toString());

    if (requestParams.sortField) {
      httpParams = httpParams.set('sortField', requestParams.sortField);
    }
    if (requestParams.sortOrder) {
      httpParams = httpParams.set('sortOrder', requestParams.sortOrder);
    }
    if (requestParams.status) {
      httpParams = httpParams.set('status', requestParams.status);
    }
    if (requestParams.targetDate) {
      httpParams = httpParams.set('targetDate', requestParams.targetDate);
    }
    if (requestParams.startDateFrom) {
      httpParams = httpParams.set('startDateFrom', requestParams.startDateFrom.toISOString());
    }
    if (requestParams.startDateTo) {
      httpParams = httpParams.set('startDateTo', requestParams.startDateTo.toISOString());
    }
    return this.http.get<ProjectsResponse>(`${this.apiUrl}/projects`, {
      params: httpParams
    });
  }

  createProject(project: Partial<GetProjectDto>): Observable<GetProjectDto> {
    project.idUser = 1; // Временно до авторизации
    return this.http.post<GetProjectDto>(`${this.apiUrl}/projects`, project);
  }
  
  updateProject(project: Partial<GetProjectDto>): Observable<GetProjectDto> {
    project.idUser = 1; // Временно до авторизации
    return this.http.put<GetProjectDto>(`${this.apiUrl}/projects`, project);
  }
}

