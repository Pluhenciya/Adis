import { Component, EventEmitter, Output } from '@angular/core';
import { ProjectStatus } from '../../models/project.model';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { NgForOf } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-filter-menu',
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatDatepickerModule,
    FormsModule,
    NgForOf,
    MatInputModule,
    MatSelectModule,
    MatButtonModule
  ],
  templateUrl: './filter-menu.component.html',
  styleUrl: './filter-menu.component.css'
})
export class FilterMenuComponent {
  @Output() applied = new EventEmitter<{status?: ProjectStatus, date?: Date}>();
  @Output() cleared = new EventEmitter<void>();

  statuses = [
    { value: ProjectStatus.Designing, viewValue: 'Проектирование' },
    { value: ProjectStatus.ContractorSearch, viewValue: 'Поиск подрядчика' },
    { value: ProjectStatus.InExecution, viewValue: 'В работе' },
    { value: ProjectStatus.Completed, viewValue: 'Завершен' }
  ];

  selectedStatus?: ProjectStatus;
  selectedDate?: Date;

  apply() {
    this.applied.emit({
      status: this.selectedStatus,
      date: this.selectedDate
    });
  }

  clear() {
    this.selectedStatus = undefined;
    this.selectedDate = undefined;
    this.cleared.emit();
  }
}
