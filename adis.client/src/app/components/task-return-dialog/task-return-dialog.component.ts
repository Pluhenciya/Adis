import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { TaskDto } from '../../models/task.model';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { CdkTextareaAutosize } from '@angular/cdk/text-field';

@Component({
  selector: 'app-task-return-dialog',
  imports: [
    MatButtonModule,
    FormsModule, 
    MatInputModule,
    MatDialogModule,
    MatFormFieldModule,
    MatIconModule,
    CdkTextareaAutosize
  ],
  templateUrl: './task-return-dialog.component.html',
  styleUrl: './task-return-dialog.component.scss'
})
export class TaskReturnDialogComponent {
  comment = '';

  constructor(
    public dialogRef: MatDialogRef<TaskReturnDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { task: TaskDto }
  ) {}

  onCancel(): void {
    this.dialogRef.close();
  }

  onReturn(): void {
    this.dialogRef.close(this.comment);
  }
}
