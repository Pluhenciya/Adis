import { Component, ViewChild, OnInit, signal, OnDestroy, AfterViewInit} from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { ProjectService } from '../../services/project.service';
import { GetProjectDto, ProjectStatus } from '../../models/project.model';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { FormsModule } from '@angular/forms';
import { MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { DatePipe, NgClass, NgForOf, NgIf } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { ProjectFormComponent } from '../../components/project-form/project-form.component';
import {MatTooltipModule} from '@angular/material/tooltip';
import {MatButtonToggleModule} from '@angular/material/button-toggle';
import { MatMenuModule, MatMenuTrigger } from '@angular/material/menu';
import { FilterMenuComponent } from '../../components/filter-menu/filter-menu.component';
import { SortMenuComponent } from '../../components/sort-menu/sort-menu.component';
import { MapService } from '../../services/map.service';

@Component({
  selector: 'app-project-list-page',
  imports: [
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatDatepickerModule,
    FormsModule,
    MatProgressSpinnerModule,
    MatTableModule,
    MatChipsModule,
    NgForOf,
    DatePipe,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatSortModule,
    MatInputModule,
    MatTooltipModule,
    MatButtonToggleModule,
    NgIf,
    MatMenuModule,
    FilterMenuComponent,
    SortMenuComponent,
  ],
  templateUrl: './project-list-page.component.html',
  styleUrls: ['./project-list-page.component.scss'],
  providers: []
})
export class ProjectListPageComponent implements OnInit, OnDestroy, AfterViewInit {

  ngOnDestroy() {
    this.observer?.disconnect();
    this.mapService.destroyMap();
  }

  private scrollTrigger: Element | null = null;
  private observer?: IntersectionObserver;
  projects: GetProjectDto[] = [];
  ProjectStatus = ProjectStatus;
  hideSingleSelectionIndicator = signal(true);
  totalCount = 0;
  isLoading = false;
 
  searchQuery = '';
  statusFilter?: ProjectStatus;
  dateFilter?: Date;
  sortField = 'startDate';
  sortOrder: 'asc' | 'desc' = 'desc';
  pageSize = 10;
  pageIndex = 0;
  hasActiveFilters = false;
  viewMode: 'list' | 'map' = 'list';
  mapCenter: [number, number] = [55.751244, 37.618423];
  zoom = 5;
  selectedProject?: GetProjectDto;
  private mapInitialized = false;

  private searchSubject = new Subject<string>();

  @ViewChild('filterTrigger') filterTrigger!: MatMenuTrigger;
  @ViewChild('sortTrigger') sortTrigger!: MatMenuTrigger;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private dialog: MatDialog,
    private projectService: ProjectService,
    private mapService: MapService
  ) {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(() => this.loadProjectsData());
  }

  ngOnInit() {
    this.loadProjectsData();
  }

  ngAfterViewInit() {
    this.setupScrollLoading();
    if (this.viewMode === 'map') {
      this.initMap();
    }
    // Обработка сортировки
    if (this.sort) {
      this.sort.sortChange.subscribe(() => {
        this.sortField = this.sort.active;
        this.sortOrder = this.sort.direction as 'asc' | 'desc';
        this.loadProjectsData();
      });
    }
  }

  private setupScrollLoading() {
    if (this.viewMode !== 'list') return;
  
    this.scrollTrigger = document.querySelector('.scroll-trigger');
    
    if (!this.scrollTrigger) {
      console.error('Scroll trigger element not found!');
      return;
    }
  
    if (this.observer) {
      this.observer.disconnect();
    }
  
    const options = {
      root: null,
      rootMargin: '0px',
      threshold: 0.1
    };
  
    this.observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting && 
            !this.isLoading && 
            this.projects.length < this.totalCount) {
          this.loadNextPage();
        }
      });
    }, options);
  
    this.observer.observe(this.scrollTrigger);
  }

  private loadNextPage() {
    this.pageIndex++;
    this.loadProjectsData(true);
  }
  loadProjectsData(append = false) {
    this.isLoading = true;
    
    const requestParams = {
      page: this.pageIndex + 1,
      pageSize: this.pageSize,
      sortField: this.sortField,
      sortOrder: this.sortOrder,
      status: this.statusFilter,
      targetDate: this.dateFilter?.toISOString().split('T')[0],
      search: this.searchQuery
    };

    this.projectService.getProjects(requestParams).subscribe({
      next: ({ projects, totalCount }) => {
        this.projects = append ? [...this.projects, ...projects] : projects;
        this.totalCount = totalCount;
        this.isLoading = false;

        if (this.viewMode === 'list') {
          setTimeout(() => this.setupScrollLoading(), 50);
        }

        if (this.viewMode === 'map') {
          const newProjects = append ? projects : this.projects;
          this.addMarkers(newProjects);
        }
      },
      error: () => this.isLoading = false
    });
  }

    // Поиск
  onSearchInput(event: Event) {
    this.searchQuery = (event.target as HTMLInputElement).value;
    this.searchSubject.next(this.searchQuery);
  }

    // Сортировка
  getSortDirectionIcon(field: string): string {
    return this.sortField === field 
      ? this.sortOrder === 'asc' ? 'arrow_upward' : 'arrow_downward' 
      : '';
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
    const dialogRef = this.dialog.open(ProjectFormComponent, {
      width: '600px',
      data: { project }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadProjectsData();
    });
  }

  applyFilters(filters: {status?: ProjectStatus, date?: Date}) {
    this.statusFilter = filters.status;
    this.dateFilter = filters.date;
    this.hasActiveFilters = !!filters.status || !!filters.date;
    this.resetAndLoad();
    this.filterTrigger.closeMenu();
  }

  clearFilters() {
    this.statusFilter = undefined;
    this.dateFilter = undefined;
    this.hasActiveFilters = false;
    this.resetAndLoad();
    this.filterTrigger.closeMenu();
  }

  applySort(sortParams: {field: string, order: 'asc' | 'desc'}) {
    this.sortField = sortParams.field;
    this.sortOrder = sortParams.order;
    this.resetAndLoad();
  }

  private resetAndLoad() {
    this.pageIndex = 0;
    this.projects = [];
    this.loadProjectsData();
  }

    updateMap() {
      if (this.viewMode === 'map') {
        this.initMap();
        this.addMarkers();
      }
    }
  
    private initMap() {
      if (!this.mapInitialized && this.viewMode === 'map') {
        const container = document.getElementById('map-container');
        if (container) {
          this.mapService.initMap(container, this.mapCenter, this.zoom);
          this.addMarkers();
          this.addMapListeners();
          this.mapInitialized = true;
        }
      }
    }
  
    private addMarkers(projects: GetProjectDto[] = this.projects) {
      projects.forEach(project => {
        if (!this.mapService.hasMarker(project.idProject)) {
          this.mapService.addMarker(project);
        }
      });
    }
  
    get projectsWithLocation() {
      return this.projects.filter(p => 
        p.location?.geometry?.coordinates?.length === 2
      );
    }

    private addMapListeners() {
      this.mapService.onMarkerClick().subscribe(project => {
        this.selectedProject = project;
      });
    }

    toggleView(mode: 'list' | 'map'): void {
      this.viewMode = mode;
      if (mode === 'map') {
        setTimeout(() => this.initMap(), 50);
      } else {
        setTimeout(() => {
          this.setupScrollLoading();
          this.scrollToTop();
        }, 100);
      }
      this.selectedProject = undefined;
    }

    private scrollToTop() {
      if (this.viewMode === 'list') {
        const container = document.querySelector('.project-cards');
        container?.scrollTo({ top: 0 });
      }
    }
}