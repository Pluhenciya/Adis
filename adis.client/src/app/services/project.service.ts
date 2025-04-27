import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Project } from '../models/project.model';
import { environment } from '../environments/environment';

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
    targetDate?: Date;
    startDateFrom?: Date;
    startDateTo?: Date;
  }): Observable<{ projects: Project[]; totalCount: number }> {
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
      httpParams = httpParams.set('targetDate', requestParams.targetDate.toISOString());
    }
    if (requestParams.startDateFrom) {
      httpParams = httpParams.set('startDateFrom', requestParams.startDateFrom.toISOString());
    }
    if (requestParams.startDateTo) {
      httpParams = httpParams.set('startDateTo', requestParams.startDateTo.toISOString());
    }

    return this.http.get<{ projects: Project[]; totalCount: number }>(`${this.apiUrl}/projects`, {
      params: httpParams
    });
  }
}