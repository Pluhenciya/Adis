import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-complete-contractor-search-dialog',
  imports: [
    ReactiveFormsModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatDialogModule,
    MatButtonModule,
    MatDatepickerModule,
    MatIconModule
  ],
  templateUrl: './complete-contractor-search-dialog.component.html',
  styleUrl: './complete-contractor-search-dialog.component.scss'
})
export class CompleteContractorSearchDialogComponent {
  contractForm = new FormGroup({
    contractor: new FormControl('', Validators.required),
    startDate: new FormControl('', Validators.required),
    endDate: new FormControl('', Validators.required)
  });

  constructor(
    public dialogRef: MatDialogRef<CompleteContractorSearchDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  onSubmit() {
    if (this.contractForm.valid) {
      this.dialogRef.close(this.contractForm.value);
    }
  }
}
