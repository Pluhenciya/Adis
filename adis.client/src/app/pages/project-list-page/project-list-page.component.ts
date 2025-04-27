import { Component, ViewChild, OnInit } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { ProjectService } from '../../services/project.service';
import { Project } from '../../models/project.model';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { FormsModule } from '@angular/forms';
import { MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { CurrencyPipe, DatePipe, NgClass, NgIf, TitleCasePipe } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

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
    TitleCasePipe,
    DatePipe,
    MatPaginatorModule,
    CurrencyPipe,
    MatButtonModule
  ],
  templateUrl: './project-list-page.component.html',
  styleUrls: ['./project-list-page.component.css']
})
export class ProjectListPageComponent implements OnInit {
  displayedColumns: string[] = ['name', 'status', 'createdAt', 'budget'];
  dataSource = new MatTableDataSource<Project>();
  totalItems = 0;
  isLoading = true;
  statusFilter = '';
  dateFilter: Date | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private projectService: ProjectService) {}

  ngOnInit() {
    this.loadProjects();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    
    if (this.sort) {
      this.dataSource.sort = this.sort;
      this.sort.sortChange.subscribe(() => {
        if (this.paginator) this.paginator.pageIndex = 0;
        this.loadProjects();
      });
    }
  }

  loadProjects() {
    this.isLoading = true;
    
    const requestParams = {
      page: this.paginator?.pageIndex + 1 || 1,
      pageSize: this.paginator?.pageSize || 10,
      sortField: this.sort?.active || 'startdate',
      sortOrder: this.sort?.direction || 'desc',
      status: this.statusFilter,
      targetDate: this.dateFilter ?? undefined,
      startDateFrom: undefined, 
      startDateTo: undefined   
    };

    this.projectService.getProjects(requestParams).subscribe({
      next: ({ projects, totalCount }) => {
        this.dataSource.data = projects;
        this.totalItems = totalCount;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        // Обработка ошибок
      }
    });
  }

  applyFilter() {
    if (this.paginator) this.paginator.pageIndex = 0;
    this.loadProjects();
  }

  resetFilters() {
    this.statusFilter = '';
    this.dateFilter = null;
    this.applyFilter();
  }
}