import { DatePipe, NgForOf, NgIf } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { TaskDto } from '../../models/task.model';
import { ProjectStatus } from '../../models/project.model';
import { TaskService } from '../../services/task.service';
import { MatDialog } from '@angular/material/dialog';
import { TaskDetailsDialogComponent } from '../task-details-dialog/task-details-dialog.component';

@Component({
  selector: 'app-task-card',
  imports: [MatCardModule, MatIconModule, NgForOf, NgIf, DatePipe],
  templateUrl: './task-card.component.html',
  styleUrl: './task-card.component.scss'
})
export class TaskCardComponent {
  @Input() task!: TaskDto;
  @Input() projectStatus!: ProjectStatus;
  @Output() taskUpdated = new EventEmitter<TaskDto>();

  constructor(
    private taskService: TaskService,
    private dialog: MatDialog
  ) {}

  handleClick() {
    this.taskService.getTaskDetails(this.task.idTask).subscribe(fullTask => {
      const dialogRef = this.dialog.open(TaskDetailsDialogComponent, {
        maxWidth: '900px',
        data: { 
          task: fullTask,
          projectStatus: this.projectStatus 
        },
        panelClass: 'task-details-dialog'
      });
  
      dialogRef.afterClosed().subscribe(updatedTask => {
          this.taskUpdated.emit(updatedTask);
      });
    });
  }

  isOverdue(): boolean {
    if (!this.task.plannedEndDate || this.task.status === 'Completed') {
      return false;
    }
    const now = new Date();
    const endDate = new Date(this.task.plannedEndDate);
    return endDate < now;
  }

  hasActualDate(): boolean {
    return !!this.task.actualEndDate;
  }
}
