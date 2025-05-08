  import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
  import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
  import { ProjectService } from '../../services/project.service';
  import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
  import { GetProjectDto, ProjectStatus } from '../../models/project.model';
  import { MatFormFieldModule } from '@angular/material/form-field';
  import { MatInputModule } from '@angular/material/input';
  import { MatDatepickerModule } from '@angular/material/datepicker';
  import { NgForOf, NgIf } from '@angular/common';
  import { MatButtonModule } from '@angular/material/button';
  import { formatISO, parseISO } from 'date-fns';
import { MatOptionModule } from '@angular/material/core';
import { MapService } from '../../services/map.service';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';

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
      MatButtonModule,
      MatOptionModule,
      NgForOf,
      MatSelectModule,
      MatIconModule
    ],
    templateUrl: './project-form.component.html',
    styleUrl: './project-form.component.scss'
  })
  export class ProjectFormComponent implements OnInit {
    projectStatuses = Object.values(ProjectStatus);
    @ViewChild('mapContainer') mapContainer!: ElementRef;
    isDrawing = false;
    projectForm: FormGroup;
    isSubmitting = false;
    errorMessage = '';

    get status() { return this.projectForm.get('status'); }
    get responsiblePerson() { return this.projectForm.get('responsiblePerson'); }
    get nameWorkObject() { return this.projectForm.get('nameWorkObject'); }
    get contractor() { return this.projectForm.get('contractor'); }
    get startDate() { return this.projectForm.get('startDate'); }
    get endDate() { return this.projectForm.get('endDate'); }
    get locationType() { return this.projectForm.get('location.type'); }
    get locationCoordinates() { return this.projectForm.get('location.coordinates'); }

    constructor(
      private fb: FormBuilder,
      private projectService: ProjectService,
      private dialogRef: MatDialogRef<ProjectFormComponent>,
      @Inject(MAT_DIALOG_DATA) public data: { project?: GetProjectDto },
      private mapService: MapService
    ) {
      this.projectForm = this.fb.group({
        status: ['', Validators.required],
        responsiblePerson: ['', Validators.required],
        nameWorkObject: ['', Validators.required],
        contractor: ['', Validators.required],
        startDate: [null, Validators.required],
        endDate: [null, Validators.required],
        location: this.fb.group({
          type: ['', Validators.required],
          coordinates: [null, Validators.required]
        })
      }, { validators: this.dateValidator });
    }

    async ngAfterViewInit() {
      await this.mapService.initMapAsync(this.mapContainer.nativeElement, [55.75, 37.57], 10);
      this.loadExistingGeometry();
    }

    private loadExistingGeometry() {
      const geometry = this.projectForm.get('location')?.value;
      if (geometry?.type && geometry?.coordinates) {
        this.mapService.addGeometry(geometry);
      }
    }

    async startDrawing(geometryType: string) {
      this.isDrawing = true;
      try {
        const geometry = await this.mapService.startDrawing(geometryType as any);
        this.projectForm.patchValue({
          location: {
            type: geometry.type,
            coordinates: geometry.coordinates
          }
        });
        this.loadExistingGeometry();
      } catch (err) {
        console.error('Ошибка рисования:', err);
      } finally {
        this.isDrawing = false;
      }
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
          idProject : this.data.project.idProject
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

    getStatusLabel(status: string) {
      const statusMap: {[key: string]: string} = {
        'Designing': 'Проектирование',
        'ContractorSearch': 'Поиск подрядчика',
        'InExecution': 'В работе',
        'Completed': 'Завершен'
      };
      return statusMap[status] || 'Неизвестный статус';
    }
  }
