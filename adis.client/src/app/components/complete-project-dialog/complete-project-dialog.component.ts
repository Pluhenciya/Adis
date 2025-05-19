import { Component, Inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { DocumentDto } from '../../models/document.model';
import { DocumentService } from '../../services/document.service';
import { MatListModule } from '@angular/material/list';
import { FormsModule } from '@angular/forms';
import { NgForOf, NgIf } from '@angular/common';
import { MatRadioModule } from '@angular/material/radio';

@Component({
  selector: 'app-complete-project-dialog',
  imports: [
    MatDialogModule,
    MatIconModule,
    MatButtonModule,
    MatListModule,
    FormsModule,
    NgIf,
    MatRadioModule,
    NgForOf
  ],
  templateUrl: './complete-project-dialog.component.html',
  styleUrl: './complete-project-dialog.component.scss'
})
export class CompleteProjectDialogComponent {
  documents: DocumentDto[] = [];
  selectedDocument: number[] = [];

  constructor(
    public dialogRef: MatDialogRef<CompleteProjectDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { projectId: number },
    private documentService: DocumentService
  ) {
    this.loadDocuments();
  }

  private loadDocuments(): void {
    this.documentService.getDocumentsByIdProject(this.data.projectId).subscribe({
      next: (docs) => this.documents = docs,
      error: () => this.documents = []
    });
  }

  getFileIcon(filename: string): string {
    const ext = filename.split('.').pop()?.toLowerCase();
    switch(ext) {
      case 'pdf': return 'picture_as_pdf';
      case 'doc':
      case 'docx': return 'description';
      case 'xls':
      case 'xlsx': return 'table_chart';
      default: return 'insert_drive_file';
    }
  }

  getFileType(filename: string): string {
    const ext = filename.split('.').pop()?.toUpperCase();
    return ext ? `${ext} файл` : 'Документ';
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.selectedDocument.length > 0) {
      this.dialogRef.close({ documentId: this.selectedDocument[0] });
    }
  }
}