import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { HttpClient } from '@angular/common/http';
import { CommentDto, PostCommentDto } from '../models/comment.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {
   }

   addComment(comment: PostCommentDto) : Observable<CommentDto>{
    return this.http.post<CommentDto>(`${this.apiUrl}/comments`, comment)
   }
}
