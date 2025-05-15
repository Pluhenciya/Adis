import { Component, OnInit } from '@angular/core';
import { TaskDto, TaskStatus } from '../../models/task.model';
import { TaskService } from '../../services/task.service';
import { AuthStateService } from '../../services/auth-state.service';
import { MatDialog } from '@angular/material/dialog';
import { TaskDetailsDialogComponent } from '../../components/task-details-dialog/task-details-dialog.component';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { DatePipe, NgForOf } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { TaskCardComponent } from '../../components/task-card/task-card.component';

@Component({
  selector: 'app-task-board-page',
  imports: [
    MatCardModule,
    MatIconModule,
    NgForOf,
    MatButtonModule,
    TaskCardComponent
  ],
  templateUrl: './task-board-page.component.html',
  styleUrl: './task-board-page.component.scss'
})
export class TaskBoardPageComponent implements OnInit {
  allTasks: TaskDto[] = [];
  pendingReviewTasks: TaskDto[] = [];
  
  get todoTasks(): TaskDto[] {
    return this.allTasks.filter(t => t.status === TaskStatus.ToDo);
  }

  get doingTasks(): TaskDto[] {
    return this.allTasks.filter(t => t.status === TaskStatus.Doing);
  }

  get checkingTasks(): TaskDto[] {
    return this.allTasks.filter(t => t.status === TaskStatus.Checking);
  }

  get completedTasks(): TaskDto[] {
    return this.allTasks.filter(t => t.status === TaskStatus.Completed);
  }

  constructor(
    private taskService: TaskService,
    private authState: AuthStateService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.taskService.getUserTasks().subscribe(tasks => {
      this.allTasks = tasks;
    });
  }

  getStatusLabel(status: TaskStatus): string {
    switch(status) {
      case TaskStatus.ToDo: return 'К выполнению';
      case TaskStatus.Doing: return 'В работе';
      case TaskStatus.Checking: return 'На проверке';
      case TaskStatus.Completed: return 'Завершена';
      default: return '';
    }
  }

  openTaskDetails(task: TaskDto): void {
    this.dialog.open(TaskDetailsDialogComponent, {
      width: '800px',
      data: { task, projectStatus: 'InExecution' }
    });
  }

  submitForReview(task: TaskDto): void {
    this.taskService.updateTaskStatus(task.idTask, TaskStatus.Checking)
      .subscribe(() => this.loadTasks());
  }

  rejectTask(task: TaskDto): void {
    this.taskService.updateTaskStatus(task.idTask, TaskStatus.Doing)
      .subscribe(() => this.loadTasks());
  }

  approveTask(task: TaskDto): void {
    this.taskService.updateTaskStatus(task.idTask, TaskStatus.Completed)
      .subscribe(() => this.loadTasks());
  }

  handleTaskUpdate(updatedTask: TaskDto): void {
    const index = this.allTasks.findIndex(t => t.idTask === updatedTask.idTask);
    if (index !== -1) {
      this.allTasks[index] = updatedTask;
    }
  }

  acceptTask(task: TaskDto): void {
    this.taskService.updateTaskStatus(task.idTask, TaskStatus.Doing)
      .subscribe(() => this.loadTasks());
  }
}
