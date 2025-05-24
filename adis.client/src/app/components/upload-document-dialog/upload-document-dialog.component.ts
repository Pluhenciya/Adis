import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { DocumentService } from '../../services/document.service';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { NgForOf, NgIf } from '@angular/common';
import { DocumentType } from '../../models/document.model';

@Component({
  selector: 'app-upload-document-dialog',
  imports: [
    MatDialogModule,
    MatButtonModule,
    MatInputModule,
    FormsModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatIconModule,
    MatSelectModule,
    NgForOf,
    NgIf
  ],
  templateUrl: './upload-document-dialog.component.html',
  styleUrl: './upload-document-dialog.component.scss'
})
export class UploadDocumentDialogComponent {
  uploadForm!: FormGroup;
  selectedFile?: File;
  errorMessage = '';
  documentTypes = Object.values(DocumentType);

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<UploadDocumentDialogComponent>,
    private documentService: DocumentService
  ) {
    this.buildForm()
  }

  buildForm(): void {
    this.uploadForm = this.fb.group({
      documentType: ['', Validators.required],
      file: [null]
    });
  }

  onFileSelected(event: any): void {
    this.selectedFile = event.target.files[0];
  }

  onSubmit(): void {
    if (this.uploadForm.valid && this.selectedFile) {
      this.documentService.uploadDocument(
        this.selectedFile, 
        undefined,
        this.uploadForm.value.documentType
      ).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => this.errorMessage = err.message || 'Ошибка загрузки'
      });
    }
  }
}
