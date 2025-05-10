import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GetProjectDto, PostProjectDto, ProjectStatus } from '../models/project.model';
import { environment } from '../environments/environment';
import { formatISO } from 'date-fns';

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
    idUser?: string | null;
  }): Observable<ProjectsResponse> {
    let params = new HttpParams()
      .set('page', requestParams.page.toString())
      .set('pageSize', requestParams.pageSize.toString())
      .set('idUser', requestParams.idUser || '');
  
    if (requestParams.sortField) params = params.set('sortField', requestParams.sortField);
    if (requestParams.sortOrder) params = params.set('sortOrder', requestParams.sortOrder);
    if (requestParams.status) params = params.set('status', requestParams.status);
    if (requestParams.targetDate) params = params.set('targetDate', requestParams.targetDate);
    if (requestParams.search?.trim()) params = params.set('search', requestParams.search.trim());
  
    return this.http.get<ProjectsResponse>(`${this.apiUrl}/projects`, { params });
  }

  createProject(project: PostProjectDto): Observable<GetProjectDto> {
    return this.http.post<GetProjectDto>(`${this.apiUrl}/projects`, {
      ...project,
      startDate: this.formatDate(project.startDate),
      endDate: this.formatDate(project.endDate),
      startExecutionDate: project.startExecutionDate ? this.formatDate(project.startExecutionDate) : null,
      endExecutionDate: project.endExecutionDate ? this.formatDate(project.endExecutionDate) : null
    });
  }

  private formatDate(date: Date): string {
    return formatISO(date, { representation: 'date' });
  }

  updateProject(project: Partial<PostProjectDto>): Observable<GetProjectDto> {
    return this.http.put<GetProjectDto>(`${this.apiUrl}/projects`, {
      ...project,
      startDate: project.startDate ? this.formatDate(project.startDate) : null,
      endDate: project.endDate ? this.formatDate(project.endDate) : null,
      startExecutionDate: project.startExecutionDate ? this.formatDate(project.startExecutionDate) : null,
      endExecutionDate: project.endExecutionDate ? this.formatDate(project.endExecutionDate) : null
    });
  }

  deleteProject(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/projects/${id}`);
  }
}

