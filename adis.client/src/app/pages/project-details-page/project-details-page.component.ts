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
import { TaskDto, TaskStatus } from '../../models/task.model';
import { ConfirmationDialogComponent } from '../../components/confirmation-dialog/confirmation-dialog.component';
import { ProjectService } from '../../services/project.service';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { switchMap } from 'rxjs';
import {MatProgressBarModule} from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MapService } from '../../services/map.service';
import { MatTooltipModule } from '@angular/material/tooltip';

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
    MatTooltipModule
  ],
  templateUrl: './project-details-page.component.html',
  styleUrl: './project-details-page.component.scss'
})
export class ProjectDetailsPageComponent implements OnInit, OnDestroy {
  project!: GetProjectWithTasksDto;
  showMap = false;
  hasGeoData = false;
  @ViewChild('projectMapRef', { static: false }) projectMapRef!: ElementRef;

  ProjectStatus = ProjectStatus;

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
    private projectService: ProjectService
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
      });
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
      width: '800px',
      data: { project }
    });
  }

  async deleteProject(project: GetProjectDto) {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: { projectName: project.name }
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
}
