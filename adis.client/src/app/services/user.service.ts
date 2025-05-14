import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserDto } from '../models/user.model';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = `${environment.apiUrl}/users`;

  constructor(private http: HttpClient) { }

  // Получить всех пользователей
  getUsers(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(this.apiUrl);
  }

  // Добавить нового пользователя
  addUser(user: UserDto): Observable<UserDto> {
    return this.http.post<UserDto>(this.apiUrl, user);
  }

  // (Опционально) Обновить пользователя
  updateUser(user: UserDto): Observable<UserDto> {
    return this.http.put<UserDto>(`${this.apiUrl}/${user.id}`, user);
  }

  // (Опционально) Удалить пользователя
  deleteUser(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  searchProjectManagers(search: string) {
    return this.http.get<any[]>(
      `${this.apiUrl}/ProjectManager/${encodeURIComponent(search)}`
    );
  }

  searchProjecters(search: string){
    return this.http.get<any[]>(
      `${this.apiUrl}/Projecter/${encodeURIComponent(search)}`
    );
  }

  getUserById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }
}
