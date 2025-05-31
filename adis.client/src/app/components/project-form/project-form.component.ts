import { AfterViewInit, Component, ElementRef, Inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, FormGroupDirective, FormsModule, NgForm, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ProjectService } from '../../services/project.service';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { GetProjectDto, PostProjectDto, ProjectStatus } from '../../models/project.model';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { NgForOf, NgIf } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { formatISO, parseISO } from 'date-fns';
import { ErrorStateMatcher, MatOptionModule } from '@angular/material/core';
import { MapService } from '../../services/map.service';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import {MatAutocompleteModule} from '@angular/material/autocomplete';
import { UserService } from '../../services/user.service';
import { Subject, debounceTime, distinctUntilChanged, filter, takeUntil, tap } from 'rxjs';
import { UserDto } from '../../models/user.model';
import { HasRoleDirective } from '../../directives/has-role.directive';
import { AuthStateService } from '../../services/auth-state.service';
import {MatStepper, MatStepperModule} from '@angular/material/stepper';

@Component({
  selector: 'app-project-form',
  imports: [
    MatDialogModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatButtonModule,
    MatOptionModule,
    NgForOf,
    MatSelectModule,
    MatIconModule,
    MatAutocompleteModule,
    HasRoleDirective,
    MatStepperModule
  ],
  templateUrl: './project-form.component.html',
  styleUrl: './project-form.component.scss',
})
export class ProjectFormComponent implements OnInit, AfterViewInit, OnDestroy {
  projectStatuses = Object.values(ProjectStatus);
  @ViewChild('mapContainer', { static: false }) mapContainer!: ElementRef;
  isDrawing = false;
  projectForm!: FormGroup;
  isSubmitting = false;
  errorMessage = '';
  projectManagers: any[] = [];
  geometryTypes = [
    { value: 'Point', label: 'Точка', icon: 'fiber_manual_record' },
    { value: 'LineString', label: 'Линия', icon: 'show_chart' },
    { value: 'Polygon', label: 'Полигон', icon: 'texture' }
  ];
  activeGeometryType: string | null = null;
  currentStep = 0;
  private destroy$ = new Subject<void>();
  private mapInitialized = false;

  basicInfo!: FormGroup;
  objectInfo!: FormGroup;
  adminInfo!: FormGroup;

  @ViewChild('stepper') stepper!: MatStepper;

  get name() { return this.basicInfo.get('name'); }
  get designStartDate() { return this.basicInfo.get('designStartDate'); }
  get designEndDate() { return this.basicInfo.get('designEndDate'); }
  
  get nameWorkObject() { return this.objectInfo.get('nameWorkObject'); }
  get locationType() { return this.objectInfo.get('location.type'); }
  get locationCoordinates() { return this.objectInfo.get('location.coordinates'); }

  get status() { return this.adminInfo.get('status'); }
  get responsiblePerson() { return this.adminInfo.get('responsiblePerson'); }
  get contractor() { return this.adminInfo.get('contractorName'); }

  constructor(
    private fb: FormBuilder,
    private projectService: ProjectService,
    private dialogRef: MatDialogRef<ProjectFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { project?: GetProjectDto },
    private mapService: MapService,
    private userService: UserService,
    private authState: AuthStateService
  ) {
    this.createForms();
  }

private createForms(): void {
    // Создаем основную форму с правильными именами групп
    this.projectForm = this.fb.group({
      basicInfo: this.fb.group({
        name: ['', Validators.required],
        designStartDate: [null, Validators.required],
        designEndDate: [null, Validators.required]
      }),
      objectInfo: this.fb.group({
        nameWorkObject: ['', Validators.required],
        location: this.fb.group({
          type: ['', Validators.required],
          coordinates: [null, Validators.required]
        })
      }),
      adminInfo: this.fb.group({
        status: [ProjectStatus.Designing],
        responsiblePerson: [null],
        contractorName: [''],
        executionStartDate: [null],
        executionEndDate: [null]
      }, { validators: [this.executionValidator] })
    }, { validators: [this.dateValidator] });

    // Инициализируем ссылки на группы
    this.basicInfo = this.projectForm.get('basicInfo') as FormGroup;
    this.objectInfo = this.projectForm.get('objectInfo') as FormGroup;
    this.adminInfo = this.projectForm.get('adminInfo') as FormGroup;
  }

  private executionValidator (control: AbstractControl): ValidationErrors | null {
      const status = control.get('status')?.value;
      const contractorName = control.get('contractorName')?.value;
      const executionStartDate = control.get('executionStartDate')?.value;
  
    const errors: {[key: string]: any} = {};
  
    if (['InExecution', 'Completed'].includes(status)) {
      if (!contractorName?.trim()) {
        errors['contractorRequired'] = true;
      }
      if (!executionStartDate) {
        errors['executionStartRequired'] = true;
      }
    }
  
    return Object.keys(errors).length ? errors : null;
  };

  basicInfoValid(): boolean {
    return this.basicInfo?.valid ?? false;
  }
  
  objectInfoValid(): boolean {
    return this.objectInfo?.valid ?? false;
  }
  

  private loadExistingGeometry() {
    const geometry = this.objectInfo.get('location')?.value;
    if (geometry?.type && geometry?.coordinates) {
      this.mapService.addGeometry(geometry);
    }
  }

  async startDrawing(geometryType: string) {
    this.activeGeometryType = geometryType;
    this.isDrawing = true;
    try {
      const geometry = await this.mapService.startDrawing(geometryType as any);
      this.objectInfo.patchValue({
          location: {
            type: geometry.type,
            coordinates: geometry.coordinates
          }
      });
      this.loadExistingGeometry();
      console.log(this.projectForm.value.objectInfoForm)
    } catch (err) {
      console.error('Ошибка рисования:', err);
    } finally {
      this.isDrawing = false;
      this.activeGeometryType = null;
    }
  }

  ngOnInit(): void {
    this.initForm();
  }

  private initForm(): void {
    this.loadExistingProjectData();
    this.setupResponsiblePersonSearch();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
  
  ngAfterViewInit(): void {
    this.stepper.selectionChange
      .pipe(takeUntil(this.destroy$))
      .subscribe(event => {
        if (event.selectedIndex === 1 && !this.mapInitialized) {
          this.initializeMap();
        }
      });
  }

  private loadExistingProjectData(): void {
    if (this.data?.project) {
      const initialData = {
        basicInfo: {
          name: this.data.project.name,
          designStartDate: this.convertToDate(this.data.project.startDate),
          designEndDate: this.convertToDate(this.data.project.plannedEndDate)
        },
        objectInfo: {
          nameWorkObject: this.data.project.workObject.name,
          location: {
            type: this.data.project.workObject.geometry.type,
            coordinates: this.data.project.workObject.geometry.coordinates
          }
        },
        adminInfo: {
          status: this.data.project.status,
          responsiblePerson: this.loadManagerData(this.data.project.idUser),
          contractorName: this.data.project.contractorName,
          executionStartDate: this.data.project.startExecutionDate,
          executionEndDate: this.data.project.plannedEndExecutionDate
        }
      };
      
      this.projectForm.patchValue(initialData, { emitEvent: false });
    }
  }
  
  private setupResponsiblePersonSearch(): void {
    if (this.authState.isAdmin()) {
      this.responsiblePerson?.valueChanges
        .pipe(
          filter(value => typeof value === 'string'),
          debounceTime(300),
          distinctUntilChanged(),
          filter(value => value.length > 2)
        )
        .subscribe(value => this.onSearchProjectManagers(value));
    }
  }

  private async initializeMap(): Promise<void> {
    try {
      if (!this.mapContainer?.nativeElement) {
        console.error('Контейнер карты не найден в DOM');
        return;
      }

      await this.mapService.initMapAsync(
        this.mapContainer.nativeElement,
        [64.539912, 40.515600],
        10
      );
      
      this.loadExistingGeometry();
      this.mapInitialized = true;
    } catch (err) {
      console.error('Ошибка инициализации карты:', err);
    }
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

  private dateValidator(form: FormGroup): ValidationErrors | null {
    const errors: any = {};
    const basicInfo = form.get('basicInfoForm');
    const adminInfo = form.get('adminInfoForm');
  
    const designStart = basicInfo?.get('designStartDate')?.value;
    const designEnd = basicInfo?.get('designEndDate')?.value;
    const execStart = adminInfo?.get('executionStartDate')?.value;
    const execEnd = adminInfo?.get('executionEndDate')?.value;

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

    const formValue = this.projectForm.value;

    const projectData: PostProjectDto = {
      name: formValue.basicInfo.name,
      status: formValue.adminInfo.status,
      idUser: formValue.adminInfo.responsiblePerson?.id,
      startDate: formValue.basicInfo.designStartDate,
      plannedEndDate: formValue.basicInfo.designEndDate,
      workObject: {
        name: formValue.objectInfo.nameWorkObject,
        geometry: {
          type: formValue.objectInfo.location.type,
          coordinates: formValue.objectInfo.location.coordinates
        }
      },
      contractorName: formValue.adminInfo.contractorName,
      startExecutionDate: formValue.adminInfo.executionStartDate,
      plannedEndExecutionDate: formValue.adminInfo.executionEndDate
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
