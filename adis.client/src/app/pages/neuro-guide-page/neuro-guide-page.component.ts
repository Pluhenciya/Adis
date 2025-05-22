import { NgForOf, NgIf } from '@angular/common';
import { Component, SecurityContext } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { NeuralGuideService } from '../../services/neural-guide.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import * as marked from 'marked';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { DocumentService } from '../../services/document.service';

@Component({
  selector: 'app-neuro-guid-page',
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    NgForOf,
    NgIf,
    MatProgressSpinnerModule
  ],
  templateUrl: './neuro-guide-page.component.html',
  styleUrl: './neuro-guide-page.component.scss'
})
export class NeuroGuidePageComponent {
  messages: { text: SafeHtml, isUser: boolean }[];
  newMessage = '';
  isLoading = false;
  errorMessage = '';

  constructor(private neuralService: NeuralGuideService, private sanitizer: DomSanitizer, private documentService: DocumentService) {
    this.messages = [
    { text: this.sanitizer.bypassSecurityTrustHtml(
      'Здравствуйте! Я готов помочь с поиском нормативных документов. Задайте ваш вопрос.'
    ), isUser: false }
  ];
  }

  private async renderMarkdown(content: string): Promise<SafeHtml> {
    const withLinks = content.replace(
      /\[Источник: (.+?)\|id:(\d+)\]/g, 
      (match, fileName, id) => 
        `<a class="source-link" href="#" data-doc-id="${id}">${fileName}</a>`
    );  

    const rawHtml = await marked.parse(withLinks);
    return this.sanitizer.bypassSecurityTrustHtml(rawHtml);
  }

  async sendMessage() {
    if (!this.newMessage.trim()) return;

    const userMessage = this.newMessage;
    this.messages.push({ text: this.sanitizer.bypassSecurityTrustHtml(userMessage), isUser: true });
    this.newMessage = '';
    this.isLoading = true;
    this.errorMessage = '';

    try {
      const response = await this.neuralService.sendMessage(userMessage).toPromise();
      
      // Ожидаем результат рендеринга
      const renderedMessage = await this.renderMarkdown(response.answer);
      
      this.messages.push({
        text: renderedMessage,
        isUser: false
      });
    } catch (error) {
      this.errorMessage = 'Ошибка при получении ответа';
      console.error('API Error:', error);
    } finally {
      this.isLoading = false;
    }
  }
  handleDocClick(event: MouseEvent) {
    const link = (event.target as HTMLElement).closest('.source-link');
    if (!link) return;
  
    const docId = link.getAttribute('data-doc-id');
    if (!docId) return;
  
    this.documentService.downloadDocument(Number(docId)).subscribe({
      next: (blob: Blob) => {
        // Создаем URL для blob
        const url = window.URL.createObjectURL(blob);
        
        // Создаем скрытую ссылку для скачивания
        const a = document.createElement('a');
        a.href = url;
        a.download = link.textContent + '.pdf'; // Или получите имя файла из сервиса
        document.body.appendChild(a);
        a.click();
        
        // Очищаем ресурсы
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
      },
      error: (err) => {
        console.error('Error downloading document:', err);
        this.errorMessage = 'Ошибка при загрузке документа';
      }
    });
    
    event.preventDefault();
  }
}
