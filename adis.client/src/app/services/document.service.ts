import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { HttpClient, HttpEvent, HttpEventType, HttpParams, HttpRequest } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { DocumentDto, DocumentType } from '../models/document.model';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getDocumentsByIdProject(idProject: number) : Observable<DocumentDto[]>{
    return this.http.get<DocumentDto[]>(`${this.apiUrl}/documents/${idProject}`);
  }

  uploadDocument(file: File, taskId?: number, documentType?: DocumentType): Observable<{progress: number, result?: any}> {
    const formData = new FormData();
    formData.append('file', file);
    
    let params = new HttpParams();
    if (taskId) {
      params = params.set('idTask', taskId.toString());
    }
    if (documentType) {
      const typeKey = getEnumKeyByValue(DocumentType, documentType);
      params = params.set('documentType', typeKey!.toString())
    }

    function getEnumKeyByValue<T extends Record<string, string>>(enumObj: T, value: string): keyof T | undefined {
      return Object.keys(enumObj).find(key => enumObj[key] === value) as keyof T;
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

  getGuideDocuments(): Observable<DocumentDto[]> {
    return this.http.get<DocumentDto[]>(`${this.apiUrl}/documents/guide`);
  }

  downloadDocument(id: number): Observable<Blob> {
    return this.http.get<Blob>(`${this.apiUrl}/documents/${id}/download`);
  }

  deleteDocument(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/documents/${id}`);
  }
}
