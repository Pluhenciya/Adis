import { Component, Inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-complete-execution-dialog',
  imports: [
    MatDialogModule,
    MatButtonModule
  ],
  templateUrl: './complete-execution-dialog.component.html',
  styleUrl: './complete-execution-dialog.component.css'
})
export class CompleteExecutionDialogComponent {
  projectName: string;

  constructor(
    public dialogRef: MatDialogRef<CompleteExecutionDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { projectName: string }
  ) {
    this.projectName = data.projectName;
  }
}
