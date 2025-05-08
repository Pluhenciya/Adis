import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import {MatRadioModule} from '@angular/material/radio';

@Component({
  selector: 'app-sort-menu',
  imports: [
    MatRadioModule,
    MatButtonToggleModule,
    CommonModule,
    FormsModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './sort-menu.component.html',
  styleUrl: './sort-menu.component.css'
})
export class SortMenuComponent {
    @Input() selectedField = 'startDate';
    @Input() sortOrder: 'asc' | 'desc' = 'desc';
    @Output() applied = new EventEmitter<{field: string, order: 'asc' | 'desc'}>();
  
    toggleSort(field: string) {
      if (this.selectedField === field) {
        // Меняем направление если выбрано то же поле
        this.sortOrder = this.sortOrder === 'asc' ? 'desc' : 'asc';
      } else {
        // При выборе нового поля сбрасываем направление
        this.selectedField = field;
        this.sortOrder = 'desc';
      }
      
      this.applied.emit({
        field: this.selectedField,
        order: this.sortOrder
      });
    }
  
    getSortIcon(field: string): string {
      if (this.selectedField !== field) return 'unfold_more';
      return this.sortOrder === 'asc' ? 'arrow_upward' : 'arrow_downward';
    }
}
