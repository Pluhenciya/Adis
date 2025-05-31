import { Component, ElementRef, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import {MatGridListModule} from '@angular/material/grid-list';
import { DatePipe, NgForOf, NgIf } from '@angular/common';
import { GetProjectDto, GetProjectWithTasksDto, ProjectStatus } from '../../models/project.model';
import { HasRoleDirective } from '../../directives/has-role.directive';
import { ProjectFormComponent } from '../../components/project-form/project-form.component';
import { MatDialog } from '@angular/material/dialog';
import { ExecutionTaskDto, TaskDto, TaskStatus } from '../../models/task.model';
import { ConfirmationDialogComponent } from '../../components/confirmation-dialog/confirmation-dialog.component';
import { ProjectService } from '../../services/project.service';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { switchMap } from 'rxjs';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MapService } from '../../services/map.service';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialogModule } from '@angular/material/dialog';
import { AddTaskDialogComponent } from '../../components/add-task-dialog/add-task-dialog.component';
import { AuthStateService } from '../../services/auth-state.service';
import { TaskCardComponent } from '../../components/task-card/task-card.component';
import { CompleteProjectDialogComponent } from '../../components/complete-project-dialog/complete-project-dialog.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatListModule } from '@angular/material/list';
import {MatCheckboxModule} from '@angular/material/checkbox';
import { FormsModule } from '@angular/forms';
import { DocumentService } from '../../services/document.service';
import { DocumentDto } from '../../models/document.model';
import { environment } from '../../environments/environment';
import { WorkObjectSectionDto } from '../../models/work-object-section.model';
import { CompleteContractorSearchDialogComponent } from '../../components/complete-contractor-search-dialog/complete-contractor-search-dialog.component';
import { CompleteExecutionDialogComponent } from '../../components/complete-execution-dialog/complete-execution-dialog.component';

interface TaskColumn {
  title: string;
  status: TaskStatus;
  count: number;
  tasks: TaskDto[];
}

@Component({
  standalone: true,
  selector: 'app-project-details-page',
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatGridListModule,
    DatePipe,
    NgForOf,
    NgIf,
    HasRoleDirective,
    RouterModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatDialogModule,
    TaskCardComponent,
    MatListModule,
    MatCheckboxModule,
    FormsModule
  ],
  templateUrl: './project-details-page.component.html',
  styleUrl: './project-details-page.component.scss'
})
export class ProjectDetailsPageComponent implements OnInit, OnDestroy {
  project!: GetProjectWithTasksDto;
  showMap = false;
  hasGeoData = false;
  @ViewChild('projectMapRef', { static: false }) projectMapRef!: ElementRef;
  documents: DocumentDto[] = [];
  ProjectStatus = ProjectStatus;
  selectedDocuments: number[] = [];
  workSections: WorkObjectSectionDto[] = [];
  isAllSelected = false;

  taskColumns: TaskColumn[] = [
    {
      title: 'Надо выполнить',
      status: TaskStatus.ToDo,
      count: 0,
      tasks: []
    },
    {
      title: 'Выполняется',
      status: TaskStatus.Doing,
      count: 0,
      tasks: []
    },
    {
      title: 'На проверке',
      status: TaskStatus.Checking,
      count: 0,
      tasks: []
    },
    {
      title: 'Готово',
      status: TaskStatus.Completed,
      count: 0,
      tasks: []
    }
  ];
  constructor(
    private mapService: MapService,
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private projectService: ProjectService,
    public authService: AuthStateService,
    private documentService: DocumentService,
    private snackBar: MatSnackBar,
    ){}

    ngOnInit() {
      this.route.paramMap.pipe(
        switchMap(params => {
          const id = Number(params.get('id'));
          return this.projectService.getProjectDetails(id);
        })
      ).subscribe(project => {
        this.project = project;
        this.updateTaskColumns();
        this.checkGeoData();
        if (this.project.status !== ProjectStatus.Designing) {
          this.loadDocuments();
        }
        this.loadWorkSections();
      });
      
    }

    toggleSelectAll() {
      this.isAllSelected = !this.isAllSelected;
      if (this.isAllSelected) {
        this.selectedDocuments = this.documents.map(d => d.idDocument);
      } else {
        this.selectedDocuments = [];
      }
    }

    private loadWorkSections() {
      const sectionsMap = new Map<number, WorkObjectSectionDto>();

      this.project.executionTasks.forEach(task => {
        if (task.workObjectSection) {
          sectionsMap.set(task.workObjectSection.idWorkObjectSection, task.workObjectSection);
        }
      });
      this.workSections = Array.from(sectionsMap.values());
    }
    
    getSectionTasks(sectionId: number): ExecutionTaskDto[] {
      return this.project.executionTasks.filter(
        t => t.workObjectSection?.idWorkObjectSection === sectionId
      );
    }
    
    downloadSelectedDocuments() {
      if (this.selectedDocuments.length === 1) {
        const doc = this.documents.find(d => d.idDocument === this.selectedDocuments[0]);
        if (doc)     
        this.documentService.downloadDocument(Number(doc.idDocument)).subscribe({
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
      } else {
        const docIds = this.selectedDocuments;
        window.open(
          `${environment.apiUrl}/documents/download-zip?ids=${docIds.join(',')}`,
          '_blank'
        );
      }
    }

    completeContractorSearch(): void {
      const dialogRef = this.dialog.open(CompleteContractorSearchDialogComponent, {
        maxWidth: '800px',
        maxHeight: '800px',
      });
    
      dialogRef.afterClosed().subscribe(result => {
        console.log(result)
        if (result) {
          this.projectService.completeContractorSearch(
            this.project.idProject,
            result.contractor,
            result.startDate,
            result.endDate
          ).subscribe({
            next: (updatedProject) => {
              this.project = updatedProject;
              this.snackBar.open('Контракт успешно заключен', 'Закрыть', { duration: 3000 });
            },
            error: () => this.snackBar.open('Ошибка сохранения', 'Закрыть')
          });
        }
      });
    }

    downloadDocument(doc: DocumentDto) {
      window.open(`${environment.apiUrl}/documents/${doc.idDocument}/download`, '_blank');
    }

    private loadDocuments() {
      this.documentService.getDocumentsByIdProject(this.project.idProject)
        .subscribe(docs => this.documents = docs);
    }

    ngOnDestroy() {
      if (this.showMap) {
        this.mapService.destroyMap();
      }
    }

    private checkGeoData() {
      this.hasGeoData = !!this.project?.workObject?.geometry?.coordinates;
    }
  
    toggleProjectMap() {
      this.showMap = !this.showMap;
      
      if (this.showMap) {
        setTimeout(() => this.initProjectMap(), 100);
      } else {
        this.mapService.destroyMap();
      }
    }

    private async initProjectMap() {
      if (!this.hasGeoData) return;
  
      const geometry = this.project.workObject.geometry;
      
      try {
        await this.mapService.initMap(
          this.projectMapRef.nativeElement,
          [20.32, 20.32],
          15 // Уровень приближения
        );
        
        if (geometry?.type && geometry?.coordinates) {
          this.mapService.addGeometry(geometry);
        }
      } catch (error) {
        console.error('Ошибка инициализации карты:', error);
        this.hasGeoData = false;
      }
    }
    
    allTasksCompleted(): boolean {
      return this.project?.tasks?.every(t => t.status === TaskStatus.Completed);
    }

    private updateTaskColumns() {
      this.taskColumns = [
        {
          title: 'Надо выполнить',
          status: TaskStatus.ToDo,
          count: this.project.tasks.filter(t => t.status === TaskStatus.ToDo).length,
          tasks: this.project.tasks.filter(t => t.status === TaskStatus.ToDo)
        },
        {
          title: 'Выполняется',
          status: TaskStatus.Doing,
          count: this.project.tasks.filter(t => t.status === TaskStatus.Doing).length,
          tasks: this.project.tasks.filter(t => t.status === TaskStatus.Doing)
        },
        {
          title: 'На проверке',
          status: TaskStatus.Checking,
          count: this.project.tasks.filter(t => t.status === TaskStatus.Checking).length,
          tasks: this.project.tasks.filter(t => t.status === TaskStatus.Checking)
        },
        {
          title: 'Готово',
          status: TaskStatus.Completed,
          count: this.project.tasks.filter(t => t.status === TaskStatus.Completed).length,
          tasks: this.project.tasks.filter(t => t.status === TaskStatus.Completed)
        }
      ];
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

  openProjectForm(project?: GetProjectDto): void {
    this.dialog.open(ProjectFormComponent, {
      maxWidth: '900px',
      data: { project }
    });
  }

  async deleteProject(project: GetProjectDto) {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: { message: `Вы уверены что хотите удалить проект \"${project.name}\"`}
    });
  
    dialogRef.afterClosed().subscribe(async result => {
      if (result) {
        try {
          await this.projectService.deleteProject(project.idProject).toPromise();
        } catch (err) {
          console.error('Ошибка удаления проекта:', err);
        }
      }
    });
  }

  openAddTaskDialog(): void {
    const dialogRef = this.dialog.open(AddTaskDialogComponent, {
      maxWidth: '800px',
      data: this.project.idProject
    });
  
    dialogRef.afterClosed().subscribe(newTask => {
      if (newTask) {
        this.project.tasks.push(newTask);
        this.updateTaskColumns();
      }
    });
  }

  onTaskUpdated(updatedTask: TaskDto) {
    const index = this.project.tasks.findIndex(t => t.idTask === updatedTask.idTask);
    if (index !== -1) {
      this.project.tasks[index] = updatedTask;
      this.updateTaskColumns();
    }
  }

  openCompleteProjectDialog(): void {
    const dialogRef = this.dialog.open(CompleteProjectDialogComponent, {
      width: '600px',
      maxWidth: '600px',
      data: { projectId: this.project.idProject }
    });
  
    dialogRef.afterClosed().subscribe(result => {
      if (result?.documentId) {
        this.completeProject(result.documentId);
      }
    });
  }
  
  private completeProject(estimateId: number): void {
    this.projectService.completeDesigningProject(this.project.idProject, estimateId)
      .subscribe({
        next: (updatedProject) => {
          this.project = updatedProject;
          this.snackBar.open('Проект успешно завершен', 'Закрыть', { duration: 3000 });
        },
        error: (err) => {
          this.snackBar.open('Ошибка завершения проекта', 'Закрыть');
        }
      });
  }

  onExecutionTaskToggle(task: ExecutionTaskDto): void {
    this.projectService.updateExecutionTaskStatus(task.idExecutionTask, task.isCompleted)
      .subscribe({
        next: (updatedTask) => {
          
          // Обновляем задачу в локальном массиве
          const index = this.project.executionTasks.findIndex(t => 
            t.idExecutionTask === updatedTask.idExecutionTask
          );
          if (index !== -1) {
            this.project.executionTasks[index].isCompleted = true;
            console.log(this.project)
          }
          this.snackBar.open('Статус задачи обновлен', 'Закрыть', { duration: 2000 });
        },
        error: () => {
          // Возвращаем предыдущее состояние при ошибке
          task.isCompleted = !task.isCompleted;
          this.snackBar.open('Ошибка обновления', 'Закрыть');
        }
      });
  }

  completeProjectExecution(): void {
    const dialogRef = this.dialog.open(CompleteExecutionDialogComponent, {
      data: { projectName: this.project.name }
    });
  
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.projectService.completeProjectExecution(this.project.idProject)
          .subscribe({
            next: (updatedProject) => {
              this.project = updatedProject;
              this.snackBar.open('Проект успешно завершен', 'Закрыть', { duration: 3000 });
            },
            error: () => {
              this.snackBar.open('Ошибка завершения проекта', 'Закрыть');
            }
          });
      }
    });
  }

  allExecutionTasksCompleted(): boolean {
    return this.project?.executionTasks?.every(t => t.isCompleted);
  }

isDesigningOverdue(project: GetProjectDto): boolean {
  if (project.actualEndDate) return false;
  return new Date() > new Date(project.plannedEndDate);
}

isExecutionOverdue(project: GetProjectDto): boolean {
  if (project.actualEndExecutionDate) return false;
  return new Date() > new Date(project.plannedEndExecutionDate!);
}
}
