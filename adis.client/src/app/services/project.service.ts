import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GetProjectDto, ProjectStatus } from '../models/project.model';
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
    status?: ProjectStatus;
    targetDate?: string;
    search?: string;
  }): Observable<ProjectsResponse> {
    let params = new HttpParams()
      .set('page', requestParams.page.toString())
      .set('pageSize', requestParams.pageSize.toString());
  
    if (requestParams.sortField) params = params.set('sortField', requestParams.sortField);
    if (requestParams.sortOrder) params = params.set('sortOrder', requestParams.sortOrder);
    if (requestParams.status) params = params.set('status', requestParams.status);
    if (requestParams.targetDate) params = params.set('targetDate', requestParams.targetDate);
    if (requestParams.search) params = params.set('search', requestParams.search);
  
    return this.http.get<ProjectsResponse>(`${this.apiUrl}/projects`, { params });
  }

  createProject(project: Partial<GetProjectDto>): Observable<GetProjectDto> {
    return this.http.post<GetProjectDto>(`${this.apiUrl}/projects`, project);
  }
  
  updateProject(project: Partial<GetProjectDto>): Observable<GetProjectDto> {
    return this.http.put<GetProjectDto>(`${this.apiUrl}/projects`, project);
  }
}

