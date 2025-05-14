import { Component, Inject } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { TaskService } from '../../services/task.service';
import { UserService } from '../../services/user.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { UserDto } from '../../models/user.model';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatOptionModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { AsyncPipe, NgForOf, NgIf } from '@angular/common';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { Observable, debounceTime, distinctUntilChanged, map, of, startWith, switchMap } from 'rxjs';
import { MatChipsModule } from '@angular/material/chips';
import { PostTaskDto } from '../../models/task.model';

@Component({
  selector: 'app-add-task-dialog',
  imports: [
    ReactiveFormsModule,
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatOptionModule,
    MatIconModule,
    MatSelectModule,
    MatInputModule,
    MatButtonModule,
    NgIf,
    NgForOf,
    MatAutocompleteModule,
    AsyncPipe,
    MatChipsModule,
  ],
  templateUrl: './add-task-dialog.component.html',
  styleUrl: './add-task-dialog.component.scss'
})
export class AddTaskDialogComponent {
  taskForm: FormGroup;
  isLoading = false;
  selectedUsers: UserDto[] = [];
  performersControl = new FormControl();
  checkersControl = new FormControl();
  filteredPerformers: Observable<UserDto[]> = new Observable<UserDto[]>();
  filteredCheckers: Observable<UserDto[]> = new Observable<UserDto[]>();

  constructor(
    private fb: FormBuilder,
    private taskService: TaskService,
    private userService: UserService,
    private snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<AddTaskDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public projectId: number
  ) {
    this.taskForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(255)]],
      description: ['', Validators.required],
      performers: [[], Validators.required],
      checkers: [[], Validators.required],
      endDate: [new Date(), Validators.required]
    });
  }

  ngOnInit(): void {
    this.initAutocomplete();
  }

  ngAfterViewInit() {
    this.taskForm.get('performers')?.valueChanges.subscribe(() => {
      this.performersControl.updateValueAndValidity();
    });
    
    this.taskForm.get('checkers')?.valueChanges.subscribe(() => {
      this.checkersControl.updateValueAndValidity();
    });
  }

  private getExcludedIds(field: 'performers' | 'checkers'): number[] {
    const oppositeField = field === 'performers' ? 'checkers' : 'performers';
    return this.taskForm.get(oppositeField)?.value || [];
  }
  

  private initAutocomplete() {
    this.filteredPerformers = this.performersControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(value => {
        const search = typeof value === 'string' ? value : '';
        const excludedIds = this.getExcludedIds('performers');
        
        return search 
          ? this.userService.searchProjecters(search).pipe(
              map(users => users.filter(u => !excludedIds.includes(u.id))))
          : of([]);
      })
    );
  
    this.filteredCheckers = this.checkersControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(value => {
        const search = typeof value === 'string' ? value : '';
        const excludedIds = this.getExcludedIds('checkers');
        
        return search 
          ? this.userService.searchProjecters(search).pipe(
              map(users => users.filter(u => !excludedIds.includes(u.id))))
          : of([]);
      })
    );
  }

  addPerformer(userId: number): void {
    if (this.taskForm.get('checkers')?.value.includes(userId)) {
      this.snackBar.open('Этот пользователь уже выбран как проверяющий', 'Закрыть', {duration: 3000});
      return;
    }
    const user = this.selectedUsers.find(u => u.id === userId);
    if (!user) {
      // Загружаем полные данные пользователя
      this.userService.getUserById(userId).subscribe(fullUser => {
        this.selectedUsers.push(fullUser);
        this.addToForm(fullUser.id, 'performers');
      });
    } else {
      this.addToForm(userId, 'performers');
    }
  }
  
  addChecker(userId: number): void {
    if (this.taskForm.get('performers')?.value.includes(userId)) {
      this.snackBar.open('Этот пользователь уже выбран как исполнитель', 'Закрыть', {duration: 3000});
      return;
    }
    const user = this.selectedUsers.find(u => u.id === userId);
    if (!user) {
      this.userService.getUserById(userId).subscribe(fullUser => {
        this.selectedUsers.push(fullUser);
        this.addToForm(fullUser.id, 'checkers');
      });
    } else {
      this.addToForm(userId, 'checkers');
    }
  }

  private addToForm(userId: number, field: 'performers' | 'checkers'): void {
    const current = this.taskForm.get(field)?.value as number[];
    if (!current.includes(userId)) {
      this.taskForm.get(field)?.setValue([...current, userId]);
    }
    this[`${field}Control`].setValue('');
  }

  onSubmit(): void {
    if (this.taskForm.invalid) return;

    const taskData: PostTaskDto = {
      name: this.taskForm.value.name,  
      description: this.taskForm.value.description,
      idPerformers: this.taskForm.value.performers,
      idCheckers: this.taskForm.value.checkers,
      idProject: this.projectId,
      endDate: this.taskForm.value.endDate
    };

    this.isLoading = true;
    this.taskService.addTask(taskData).subscribe({
      next: (newTask) => {
        this.snackBar.open('Задача успешно создана', 'Закрыть', { duration: 3000 });
        this.dialogRef.close(newTask);
      },
      error: (err) => {
        this.snackBar.open('Ошибка при создании задачи', 'Закрыть');
        console.error(err);
        this.isLoading = false;
      }
    });
  }

  getUsername(userId: number): string {
    const user = this.selectedUsers.find(u => u.id === userId);
    return user?.fullName || user?.email || 'Загрузка...';
  }
  
  removePerformer(userId: number): void {
    const performers = this.taskForm.get('performers')?.value.filter((id: number) => id !== userId);
    this.taskForm.get('performers')?.setValue(performers);
  }
  
  removeChecker(userId: number): void {
    const checkers = this.taskForm.get('checkers')?.value.filter((id: number) => id !== userId);
    this.taskForm.get('checkers')?.setValue(checkers);
  }
}
