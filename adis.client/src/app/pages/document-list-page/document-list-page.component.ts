import { Component } from '@angular/core';
import { DocumentDto, DocumentType } from '../../models/document.model';
import { DocumentService } from '../../services/document.service';
import { AuthStateService } from '../../services/auth-state.service';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from '../../components/confirmation-dialog/confirmation-dialog.component';
import { MatIconModule } from '@angular/material/icon';
import { NgForOf, NgIf } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';
import { UploadDocumentDialogComponent } from '../../components/upload-document-dialog/upload-document-dialog.component';
import { ready } from 'yandex-maps';

@Component({
  selector: 'app-document-list-page',
  imports: [
    MatIconModule,
    NgIf,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatCardModule,
    MatButtonModule,
    FormsModule,
    NgForOf
  ],
  templateUrl: './document-list-page.component.html',
  styleUrl: './document-list-page.component.scss'
})
export class DocumentListPageComponent {
  documents: DocumentDto[] = [];
  filteredDocuments: DocumentDto[] = [];
  documentTypes: string[] = [];
  searchTerm = '';
  selectedType = 'all';
  isAdmin = false;

  constructor(
    private documentService: DocumentService,
    private authService: AuthStateService,
    public dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadGuideDocuments();
    this.isAdmin = this.authService.isAdmin();
  }

  private loadGuideDocuments(): void {
    this.documentService.getGuideDocuments().subscribe({
      next: (docs) => {
        this.documents = docs;
        this.applyFilters();
        this.documentTypes = this.getUniqueDocumentTypes();
      },
      error: (err) => console.error('Ошибка загрузки документов:', err)
    });
  }

  private getUniqueDocumentTypes(): string[] {
    return [...new Set(this.documents.map(d => d.documentType))];
  }

  applyFilters(): void {
    this.filteredDocuments = this.documents.filter(doc => {
      const matchesSearch = doc.fileName.toLowerCase().includes(this.searchTerm.toLowerCase());
      const matchesType = this.selectedType === 'all' || doc.documentType === this.selectedType;
      return matchesSearch && matchesType;
    });
  }

  openUploadDialog(): void {
    const dialogRef = this.dialog.open(UploadDocumentDialogComponent, {
      width: '500px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) setTimeout(() => this.loadGuideDocuments(), 1000);
    });
  }

  deleteDocument(documentId: number): void {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: { 
        title: 'Удаление документа',
        message: 'Вы уверены что хотите удалить этот документ?' 
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.documentService.deleteDocument(documentId).subscribe({
          next: () => this.loadGuideDocuments(),
          error: (err) => console.error('Error deleting document:', err)
        });
      }
    });
  }

  downloadDocument(document: DocumentDto): void {
    this.documentService.downloadDocument(Number(document.idDocument)).subscribe({
      next: ({ blob, filename }) => {
        const url = window.URL.createObjectURL(blob!);
        const a = window.document.createElement('a');
        a.href = url;
        a.download = filename; // Use server-provided filename
        window.document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        window.document.body.removeChild(a);
      },
      error: (err) => {
        console.error('Error downloading document:', err);
      }
    });
  }

  mapDocumentType(serverType: string): DocumentType {
    const typeMap: { [key: string]: DocumentType } = {
      'GOST': DocumentType.GOST,
      'SNIP': DocumentType.SNIP,
      'SP': DocumentType.SP,
      'TU': DocumentType.TU,
      'SanPin': DocumentType.SanPin,
      'Estimate': DocumentType.Estimate,
      'TechnicalRegulation': DocumentType.TechnicalRegulation,
      'Other': DocumentType.Other
    };
  
    return typeMap[serverType] || DocumentType.Other;
  }
}
