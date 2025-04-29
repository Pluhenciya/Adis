import { Component, ViewChild, OnInit } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatPaginator, MatPaginatorIntl, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { ProjectService } from '../../services/project.service';
import { Project } from '../../models/project.model';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { FormsModule } from '@angular/forms';
import { MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { DatePipe, NgClass, NgIf } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { startWith, switchMap, tap } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { ProjectFormComponent } from '../../components/project-form/project-form.component';

const rangeLabel: string = 'из';
const itemsPerPageLabel: string = 'Элементов на странице:';
const firstPageLabel: string = 'Первая страница';
const lastPageLabel: string = 'Последняя страница';
const previousPageLabel: string = 'Предыдущая страница';
const nextPageLabel: string = 'Следующая страница';

const getRangeLabel: (page: number, pageSize: number, length: number) => string = (
  page: number,
  pageSize: number,
  length: number
): string => {
  return new MatPaginatorIntl().getRangeLabel(page, pageSize, length).replace(/[a-z]+/i, rangeLabel);
};

export function getPaginatorIntl(): MatPaginatorIntl {
  const paginatorIntl: MatPaginatorIntl = new MatPaginatorIntl();

  paginatorIntl.itemsPerPageLabel = itemsPerPageLabel;
  paginatorIntl.firstPageLabel = firstPageLabel;
  paginatorIntl.lastPageLabel = lastPageLabel;
  paginatorIntl.previousPageLabel = previousPageLabel;
  paginatorIntl.nextPageLabel = nextPageLabel;
  paginatorIntl.getRangeLabel = getRangeLabel;
  
  return paginatorIntl;
}

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
    NgIf,
    NgClass,
    DatePipe,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatSortModule
  ],
  templateUrl: './project-list-page.component.html',
  styleUrls: ['./project-list-page.component.scss'],
  providers: [
    { provide: MatPaginatorIntl, useValue: getPaginatorIntl() }
 ]
})
export class ProjectListPageComponent implements OnInit {
  displayedColumns: string[] = ['name', 'status', 'dates', 'createdAt', 'budget', 'actions'];
  dataSource = new MatTableDataSource<Project>();
  pageIndex:number = 0;
  pageSize:number = 10;
  length:number = 10;
  isLoading = true;
  statusFilter = '';
  dateFilter: Date | null = null;
  
  @ViewChild('paginator') paginator!: MatPaginator; 
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private dialog: MatDialog,
    private projectService: ProjectService
  ) {}

  ngOnInit() {
  }

  ngAfterViewInit() {
    setTimeout(() => {
      if (!this.paginator || !this.sort) return;
      
      // Инициализация начальных значений
      this.paginator.pageSize = 10;
      this.paginator.pageIndex = 0;
      
      this.initializeComponents();
    }, 0);
  }

  private initializeComponents() {
    // Проверяем инициализацию компонентов
    if (!this.paginator || !this.sort) {
      console.error('Paginator or Sort not initialized!');
      return;
    }

    this.dataSource.sort = this.sort;

    // Первая загрузка данных
    this.paginator.page
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoading = true;
          return this.loadProjectsData();
        })
      )
      .subscribe();

    // Обработка сортировки
    this.sort.sortChange.subscribe(() => {
      if (this.paginator) {
        this.paginator.pageIndex = 0;
      }
      this.loadProjectsData().subscribe();
    });
  }

  loadProjectsData(event?:PageEvent) {
    console.log('--- START loadProjectsData ---');

    if (event) {
      console.log('Event from paginator:', event);
      this.pageIndex = event.pageIndex;
      this.pageSize = event.pageSize;
    }

    // Проверяем инициализацию перед использованием
    const pageIndex = this.paginator?.pageIndex ?? 0;
    const pageSize = this.paginator?.pageSize ?? 10;
    const sortField = this.sort?.active || 'createdAt';
    const sortOrder: 'asc' | 'desc' = this.sort?.direction === 'asc' ? 'asc' : 'desc';

    console.log('Current paginator state:', {
      pageIndex: this.paginator?.pageIndex,
      pageSize: this.paginator?.pageSize,
      length: this.paginator?.length
    });

    console.log('Component state:', {
      pageIndex: this.pageIndex,
      pageSize: this.pageSize,
      length: this.length
    });

    const requestParams = {
      page: pageIndex + 1,
      pageSize: pageSize,
      sortField: sortField,
      sortOrder: sortOrder,
      status: this.statusFilter,
      targetDate: this.dateFilter ? 
        this.dateFilter.toISOString().split('T')[0] : 
        undefined,
      startDateFrom: undefined,
      startDateTo: undefined
    };

    this.isLoading = true;

    console.log('Sending request with params:', requestParams);

    return this.projectService.getProjects(requestParams).pipe(
      tap({
        next: ({ projects, totalCount }) => {
          console.log('Response received:', {
            projectsCount: projects.length,
            totalCount,
            page: requestParams.page,
            pageSize: requestParams.pageSize
          });
  
          // Проверка на выход за границы
          if (pageIndex * pageSize >= totalCount) {
            const newPageIndex = Math.max(0, Math.floor(totalCount / pageSize) - 1);
            console.warn(`Correcting pageIndex from ${pageIndex} to ${newPageIndex}`);
            this.pageIndex = newPageIndex;
          }
  
          console.log('Updating component state:', {
            newPageIndex: this.pageIndex,
            newPageSize: this.pageSize,
            newLength: totalCount
          });
  
          this.dataSource.data = projects;
          this.length = totalCount;
  
          if (this.paginator) {
            console.log('Updating paginator:', {
              length: totalCount,
              pageIndex: this.pageIndex,
              pageSize: this.pageSize
            });
            
            this.paginator.length = totalCount;
            this.paginator.pageIndex = this.pageIndex;
            this.paginator.pageSize = this.pageSize;
          }
  
          this.isLoading = false;
          console.log('--- END loadProjectsData SUCCESS ---');
        },
        error: (err) => {
          console.error('Error in loadProjectsData:', err);
          this.isLoading = false;
          console.log('--- END loadProjectsData ERROR ---');
        }
      })
    );
  }


  applyFilter() {
    if (this.paginator) this.paginator.pageIndex = 0;
    this.loadProjectsData().subscribe();
  }

  resetFilters() {
    this.statusFilter = '';
    this.dateFilter = null;
    this.applyFilter();
  }

  getStatusLabel(status: string){
    switch(status.toLowerCase()){
      case 'draft':
        return 'Черновик'
      case 'inprogress':
        return 'Выполняется'
      case 'completed':
        return 'Завершен'
      case 'overdue':
        return 'Просрочен'
      default:
        return ''
    }
  }

  handlePageEvent(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadProjectsData();
  }
  
  openProjectForm(project?: Project): void {
    const dialogRef = this.dialog.open(ProjectFormComponent, {
      width: '600px',
      data: { project }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadProjectsData().subscribe();
      }
    });
  }
}