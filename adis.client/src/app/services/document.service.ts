import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { HttpClient, HttpEvent, HttpEventType, HttpParams, HttpRequest } from '@angular/common/http';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  uploadDocument(file: File, taskId?: number): Observable<{progress: number, result?: any}> {
    const formData = new FormData();
    formData.append('file', file);
    
    let params = new HttpParams();
    if (taskId) {
      params = params.set('idTask', taskId.toString());
    }

    const req = new HttpRequest(
      'POST',
      `${this.apiUrl}/documents/upload`,
      formData,
      {
        reportProgress: true,
        responseType: 'json',
        params: params
      }
    );

    return this.http.request(req).pipe(
      map(event => this.getUploadEventStatus(event))
    );
  }

  private getUploadEventStatus(event: HttpEvent<any>) {
    switch (event.type) {
      case HttpEventType.UploadProgress:
        return {
          progress: Math.round(100 * (event.loaded / (event.total || 1))),
          result: null
        };
      case HttpEventType.Response:
        return {
          progress: 100,
          result: event.body
        };
      default:
        return { progress: 0, result: null };
    }
  }
}
