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
import { TaskResultDialogComponent } from '../../components/task-result-dialog/task-result-dialog.component';
import { TaskReturnDialogComponent } from '../../components/task-return-dialog/task-return-dialog.component';

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
  currentUserId: number;

  // Задачи где пользователь исполнитель и задача не принята
  get assignedTasks(): TaskDto[] {
    return this.allTasks.filter(t => 
      t.performers.some(p => p.id === this.currentUserId) &&
      t.status === TaskStatus.ToDo
    );
  }

  // Принятые задачи в работе
  get acceptedTasks(): TaskDto[] {
    return this.allTasks.filter(t => 
      t.performers.some(p => p.id === this.currentUserId) &&
      t.status === TaskStatus.Doing
    );
  }

  // Задачи на проверке где пользователь проверяющий
  get reviewTasks(): TaskDto[] {
    return this.allTasks.filter(t => 
      t.checkers.some(c => c.id === this.currentUserId) &&
      t.status === TaskStatus.Checking
    );
  }

  constructor(
    private taskService: TaskService,
    private authState: AuthStateService,
    private dialog: MatDialog
  ) {
    this.currentUserId = Number(this.authState.currentUserId);
  }

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.taskService.getUserTasks().subscribe(tasks => {
      this.allTasks = tasks;
    });
  }

  handleTaskUpdate(updatedTask: TaskDto): void {
    this.loadTasks();
  }
}
