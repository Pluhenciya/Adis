import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { TaskDto } from '../../models/task.model';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { CdkTextareaAutosize } from '@angular/cdk/text-field';
import { NgForOf, NgIf } from '@angular/common';

@Component({
  selector: 'app-task-result-dialog',
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatInputModule,
    CdkTextareaAutosize,
    NgIf,
    NgForOf
  ],
  templateUrl: './task-result-dialog.component.html',
  styleUrl: './task-result-dialog.component.scss'
})
export class TaskResultDialogComponent {
  result = '';
  selectedFiles: File[] = [];

  constructor(
    public dialogRef: MatDialogRef<TaskResultDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { task: TaskDto }
  ) {}

  onCancel(): void {
    this.dialogRef.close();
  }

  onFileSelected(event: any): void {
    this.selectedFiles = Array.from(event.target.files);
  }

  removeFile(fileToRemove: File): void {
    this.selectedFiles = this.selectedFiles.filter(file => file !== fileToRemove);
  }

  onSubmit(): void {
    const result = {
      text: this.result,
      files: this.selectedFiles
    };
    this.dialogRef.close(result);
  }
}
