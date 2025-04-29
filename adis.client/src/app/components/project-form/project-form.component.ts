import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProjectService } from '../../services/project.service';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { Project } from '../../models/project.model';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { NgIf } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { formatISO, parseISO } from 'date-fns';

@Component({
  selector: 'app-project-form',
  imports: [
    MatDialogModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    NgIf,
    MatButtonModule
  ],
  templateUrl: './project-form.component.html',
  styleUrl: './project-form.component.scss'
})
export class ProjectFormComponent implements OnInit {
  projectForm: FormGroup;
  isSubmitting = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private projectService: ProjectService,
    private dialogRef: MatDialogRef<ProjectFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { project?: Project }
  ) {
    this.projectForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(255)]],
      budget: ['', [Validators.required, Validators.min(0)]],
      startDate: [null, Validators.required], 
      endDate: [null, Validators.required],
    }, { validators: this.dateValidator });
  }

  ngOnInit(): void {
    if (this.data?.project) {
      // Преобразуем строки в Date объекты
      const initialData = {
        ...this.data.project,
        startDate: this.convertToDate(this.data.project.startDate),
        endDate: this.convertToDate(this.data.project.endDate)
      };
      
      this.projectForm.patchValue(initialData);
    }
  }

  private convertToDate(dateString: string | Date): Date {
    if (dateString instanceof Date) return dateString;
    return parseISO(dateString);
  }

  dateValidator(form: FormGroup) {
    const start = form.get('startDate')?.value;
    const end = form.get('endDate')?.value;
    
    if (start && end && new Date(start) > new Date(end)) {
      return { dateRange: true };
    }
    return null;
  }

  onSubmit() {
    if (this.projectForm.invalid) return;

    this.isSubmitting = true;
    this.errorMessage = '';

    const formatDate = (date: Date) => 
    formatISO(date, { representation: 'date' }); // 'YYYY-MM-DD'

    const projectData = {
      ...this.projectForm.value,
      startDate: formatDate(this.projectForm.value.startDate),
      endDate: formatDate(this.projectForm.value.endDate)
    };

    const request = this.data?.project 
      ? this.projectService.updateProject({
        ...projectData,
        idProject : this.data.project.idProject,
        createdAt : this.data.project.createdAt
      })
      : this.projectService.createProject(projectData);

    request.subscribe({
      next: (project) => {
        this.dialogRef.close(project);
      },
      error: (err) => {
        this.errorMessage = err.message || 'Ошибка сохранения проекта';
        this.isSubmitting = false;
      }
    });
  }

  get name() { return this.projectForm.get('name'); }
  get budget() { return this.projectForm.get('budget'); }
  get startDate() { return this.projectForm.get('startDate'); }
  get endDate() { return this.projectForm.get('endDate'); }
}
