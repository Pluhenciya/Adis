import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProjectService } from '../../services/project.service';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { GetProjectDto, PostProjectDto, ProjectStatus } from '../../models/project.model';
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
import {MatAutocompleteModule} from '@angular/material/autocomplete';
import { UserService } from '../../services/user.service';
import { debounceTime, distinctUntilChanged, filter } from 'rxjs';
import { UserDto } from '../../models/user.model';

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
    MatIconModule,
    MatAutocompleteModule
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
  projectManagers: any[] = [];

  get name() { return this.projectForm.get('name')}
  get status() { return this.projectForm.get('status'); }
  get responsiblePerson() { return this.projectForm.get('responsiblePerson'); }
  get nameWorkObject() { return this.projectForm.get('workObject.nameWorkObject'); }
  get contractor() { return this.projectForm.get('contractorName'); }
  get designStartDate() { return this.projectForm.get('designStartDate'); }
  get designEndDate() { return this.projectForm.get('designEndDate'); }
  get locationType() { return this.projectForm.get('location.type'); }
  get locationCoordinates() { return this.projectForm.get('location.coordinates'); }
  

  constructor(
    private fb: FormBuilder,
    private projectService: ProjectService,
    private dialogRef: MatDialogRef<ProjectFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { project?: GetProjectDto },
    private mapService: MapService,
    private userService: UserService
  ) {
    this.projectForm = this.fb.group({
      // Обновленная структура формы
      name: ['', Validators.required],
      status: [ProjectStatus.Designing, Validators.required],
      responsiblePerson: [null, Validators.required],
      designStartDate: [null, Validators.required],
      designEndDate: [null, Validators.required],
      workObject: this.fb.group({
        nameWorkObject: ['', Validators.required],
        location: this.fb.group({
          type: ['', Validators.required],
          coordinates: [null, Validators.required]
        })
      }),
      contractorName: [''],
      executionStartDate: [null],
      executionEndDate: [null]
    }, { validators: this.dateValidator });
  }

  async ngAfterViewInit() {
    await this.mapService.initMapAsync(this.mapContainer.nativeElement, [55.75, 37.57], 10);
    this.loadExistingGeometry();
  }

  private loadExistingGeometry() {
    const geometry = this.projectForm.get('workObject.location')?.value;
    if (geometry?.type && geometry?.coordinates) {
      this.mapService.addGeometry(geometry);
    }
  }

  async startDrawing(geometryType: string) {
    this.isDrawing = true;
    try {
      const geometry = await this.mapService.startDrawing(geometryType as any);
      this.projectForm.patchValue({
        workObject: {
          location: { // Правильный путь до location
            type: geometry.type,
            coordinates: geometry.coordinates
          }
        }
      });
      console.log(this.projectForm)
      this.loadExistingGeometry();
    } catch (err) {
      console.error('Ошибка рисования:', err);
    } finally {
      this.isDrawing = false;
    }
  }

ngOnInit(): void {
  if (this.data?.project) {
    var user =this.loadManagerData(this.data.project.idUser)
      const initialData = {
        ...this.data.project,
        responsiblePerson: user,
        designStartDate: this.convertToDate(this.data.project.startDate),
        designEndDate: this.convertToDate(this.data.project.endDate),
        workObject: {
          nameWorkObject: this.data.project.workObject.name,
          location: {
            type: this.data.project.workObject.geometry.type,
            coordinates: this.data.project.workObject.geometry.coordinates
          }
        }
      };
      this.projectForm.patchValue(initialData, { emitEvent: false });
  }

  this.responsiblePerson?.valueChanges
    .pipe(
      filter(value => typeof value === 'string'),
      debounceTime(300),
      distinctUntilChanged(),
      filter(value => value.length > 2)
    )
    .subscribe(value => this.onSearchProjectManagers(value));
}

private async loadManagerData(idUser: number): Promise<UserDto | undefined> {
  try {
    const manager = await this.userService.getUserById(idUser).toPromise();
    if (manager) {
      this.responsiblePerson?.setValue(manager, { emitEvent: false });
    }
    return manager;
  } catch (err) {
    console.error('Ошибка загрузки менеджера:', err);
    return undefined;
  }
}

  private convertToDate(dateString: string | Date): Date {
    if (dateString instanceof Date) return dateString;
    return parseISO(dateString);
  }

  dateValidator(form: FormGroup) {
    const errors: any = {};
    const designStart = form.get('designStartDate')?.value;
    const designEnd = form.get('designEndDate')?.value;
    const execStart = form.get('executionStartDate')?.value;
    const execEnd = form.get('executionEndDate')?.value;

    if (designStart && designEnd && designStart > designEnd) {
      errors.designDateRange = true;
    }

    if (execStart && execEnd && execStart > execEnd) {
      errors.executionDateRange = true;
    }

    if (designEnd && execStart && designEnd > execStart) {
      errors.designBeforeExecution = true;
    }

    return Object.keys(errors).length ? errors : null;
  }


  onSubmit() {
    if (this.projectForm.invalid) return;

    this.isSubmitting = true;
    this.errorMessage = '';

    const projectData: PostProjectDto = {
      name: this.projectForm.value.name,
      status: this.projectForm.value.status,
      idUser: this.projectForm.value.responsiblePerson.id,
      startDate: this.projectForm.value.designStartDate,
      endDate: this.projectForm.value.designEndDate,
      workObject: {
        name: this.projectForm.value.workObject.nameWorkObject,
        geometry: {
          type: this.projectForm.value.workObject.location.type,
          coordinates: this.projectForm.value.workObject.location.coordinates
        }
      },
      contractorName: this.projectForm.value.contractorName,
      startExecutionDate: this.projectForm.value.executionStartDate,
      endExecutionDate: this.projectForm.value.executionEndDate
    };

    if (this.data?.project) {
      projectData.idProject = this.data.project.idProject;
    }

    const request = this.data?.project
      ? this.projectService.updateProject(projectData)
      : this.projectService.createProject(projectData);

    request.subscribe({
      next: (project) => {
        this.dialogRef.close(project);
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Ошибка сохранения проекта';
        this.isSubmitting = false;
      }
    });
  }

  getStatusLabel(status: string) {
    const statusMap: { [key: string]: string } = {
      'Designing': 'Проектирование',
      'ContractorSearch': 'Поиск подрядчика',
      'InExecution': 'В работе',
      'Completed': 'Завершен'
    };
    return statusMap[status] || 'Неизвестный статус';
  }

  onSearchProjectManagers(search: string) {
    if (typeof search !== 'string') return;
    this.userService.searchProjectManagers(search).subscribe({
      next: (users) => {
        this.projectManagers = users;
      },
      error: (err) => console.error('Ошибка поиска:', err)
    });
  }

  displayManager = (manager: UserDto): string => {
    return manager?.fullName || '';
  };
}
