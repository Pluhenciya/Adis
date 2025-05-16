import { Observable } from "rxjs";
import { environment } from "../environments/environment";
import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { PostTaskDto, PutTaskDto, TaskDetailsDto, TaskDto, TaskStatus } from "../models/task.model";


@Injectable({
    providedIn: 'root'
  })

export class TaskService {
    private apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) { }

    getTaskDetails(taskId: number): Observable<TaskDetailsDto> {
        return this.http.get<TaskDetailsDto>(`${this.apiUrl}/tasks/${taskId}`);
      }

    updateTask(task: PutTaskDto): Observable<TaskDetailsDto> {
        return this.http.put<TaskDetailsDto>(`${this.apiUrl}/tasks`, task);
    }

    addTask(taskData: PostTaskDto): Observable<TaskDetailsDto> {
      return this.http.post<TaskDetailsDto>(`${this.apiUrl}/tasks`, taskData);
    }

    getUserTasks(): Observable<TaskDto[]> {
      return this.http.get<TaskDto[]>(`${this.apiUrl}/tasks`);
    }

    updateTaskStatus(idTask: number, status: TaskStatus): Observable<TaskDto>{
      return this.http.get<TaskDto>(`${this.apiUrl}/tasks/${idTask}/${status}`);
    }

    submitTaskResult(idTask: number, result: string): Observable<TaskDto>{
      return this.http.put<TaskDto>(`${this.apiUrl}/tasks/${idTask}`, {result});
    }
}