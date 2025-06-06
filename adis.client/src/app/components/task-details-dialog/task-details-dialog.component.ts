import { AsyncPipe, DatePipe, NgForOf, NgIf } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import {MatListModule} from '@angular/material/list';
import { PostTaskDto, PutTaskDto, TaskDetailsDto, TaskDto, TaskStatus } from '../../models/task.model';
import { MatButtonModule } from '@angular/material/button';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { TaskService } from '../../services/task.service';
import {MatSnackBar} from '@angular/material/snack-bar';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { UserDto } from '../../models/user.model';
import { Observable, catchError, combineLatest, debounceTime, distinctUntilChanged, finalize, last, lastValueFrom, map, of, switchMap } from 'rxjs';
import { UserService } from '../../services/user.service';
import { CommentService } from '../../services/comment.service';
import { CommentDto, PostCommentDto } from '../../models/comment.model';
import { HasRoleDirective } from '../../directives/has-role.directive';
import { AuthService } from '../../services/auth.service';
import { AuthStateService } from '../../services/auth-state.service';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatSelectModule } from '@angular/material/select';
import { TaskResultDialogComponent } from '../task-result-dialog/task-result-dialog.component';
import { TaskReturnDialogComponent } from '../task-return-dialog/task-return-dialog.component';
import { DocumentService } from '../../services/document.service';
import { environment } from '../../environments/environment';

export interface TaskDialogData {
  task: TaskDetailsDto;
  projectStatus: 'Designing' | 'ContractorSearch' | 'InExecution' | 'Completed';
}

@Component({
  standalone: true,
  selector: 'app-task-details-dialog',
  imports: [
    MatIconModule,
    NgForOf,
    NgIf,
    MatDividerModule,
    MatListModule,
    MatButtonModule,
    ReactiveFormsModule,
    MatChipsModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatAutocompleteModule,
    AsyncPipe,
    DatePipe,
    HasRoleDirective,
    MatDatepickerModule,
    MatSelectModule
  ],
  templateUrl: './task-details-dialog.component.html',
  styleUrl: './task-details-dialog.component.scss',
})
export class TaskDetailsDialogComponent implements OnInit {
  taskForm: FormGroup;
  isEditing = false;
  isLoading = false;
  performersControl = new FormControl();
  checkersControl = new FormControl();
  filteredPerformers: Observable<UserDto[]> = new Observable<UserDto[]>();
  filteredCheckers: Observable<UserDto[]> = new Observable<UserDto[]>();
  allUsers: UserDto[] = [];
  commentControl = new FormControl('', [Validators.required, Validators.maxLength(1000)]);
  taskStatuses = Object.values(TaskStatus);
  currentUserId: number;
  isPerformer = false;
  isChecker = false;

  constructor(
    private fb: FormBuilder,
    private taskService: TaskService,
    public authService: AuthStateService,
    private userService: UserService,
    private commentService: CommentService,
    private snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<TaskDetailsDialogComponent>,
    private dialog: MatDialog,
    private documentService: DocumentService,
    @Inject(MAT_DIALOG_DATA) public data: TaskDialogData
  ) {
    this.taskForm = this.fb.group({
      name: [data.task.name, [Validators.required, Validators.maxLength(255)]],
      description: [data.task.description, [Validators.required, Validators.maxLength(2000)]],
      performers: [data.task.performers, [Validators.required]],
      checkers: [data.task.checkers, [Validators.required]],
      endDate: [data.task.plannedEndDate, [Validators.required]],
      status: [data.task.status]
    });
    this.currentUserId = Number(this.authService.currentUserId);
    this.checkRoles();
    console.log(data.task)
  }

  private checkRoles() {
    this.isPerformer = this.data.task.performers?.some(p => p.id === this.currentUserId);
    this.isChecker = this.data.task.checkers?.some(c => c.id === this.currentUserId);
  }

  acceptTask() {
     this.taskService.updateTaskStatus(this.data.task.idTask, TaskStatus.Doing)
       .subscribe(updatedTask => this.dialogRef.close(updatedTask));
  }

  approveTask() {
     this.taskService.updateTaskStatus(this.data.task.idTask, TaskStatus.Completed)
       .subscribe(updatedTask => this.dialogRef.close(updatedTask));
  }

  submitTaskResult(task: TaskDto, result: string): void {
     this.taskService.submitTaskResult(task.idTask, result).subscribe();
   }
 
   returnTask(task: TaskDto, comment: string): void {
    const commentDto: PostCommentDto = {
      idTask: task.idTask,
      text: comment
    };
    this.commentService.addComment(commentDto).subscribe()
    this.taskService.updateTaskStatus(this.data.task.idTask, TaskStatus.ToDo)
       .subscribe(updatedTask => this.dialogRef.close(updatedTask));
   }

   openResultDialog(task: TaskDto): void {
    const dialogRef = this.dialog.open(TaskResultDialogComponent, {
      width: '600px',
      data: { task }
    });
  
    dialogRef.afterClosed().subscribe(async (result: { text?: string; files?: File[] }) => {
      if (result?.files?.length || result?.text) {
        try {
          // Обновляем статус задачи независимо от наличия текста
          const updateStatus$ = this.taskService.updateTaskStatus(task.idTask, TaskStatus.Checking);
          
          // Загрузка файлов если есть
          const uploadFiles$ = result.files?.length 
            ? this.uploadFiles(result.files, task.idTask)
            : of([]);
  
          // Отправка текста если есть
          const submitText$ = result.text 
            ? this.taskService.submitTaskResult(task.idTask, result.text)
            : of(null);
  
          await lastValueFrom(
            combineLatest([uploadFiles$, submitText$, updateStatus$]).pipe(
              finalize(() => {
                this.dialogRef.close(true); // Закрываем главный диалог
                this.snackBar.open('Результат сохранен', 'Закрыть', { duration: 3000 });
              })
            )
          );
        } catch (error) {
          this.snackBar.open('Ошибка сохранения', 'Закрыть');
        }
      }
    });
  }

  getDocumentUrl(documentId: number): string {
    return `${environment.apiUrl}/documents/${documentId}/download`;
  }
  
  getFileType(filename: string): string {
    const ext = filename.split('.').pop()?.toLowerCase();
    switch(ext) {
      case 'pdf': return 'PDF Документ';
      case 'doc':
      case 'docx': return 'Word Документ';
      case 'xls':
      case 'xlsx': return 'Excel Документ';
      case 'jpg':
      case 'jpeg':
      case 'png': return 'Картинка';
      default: return 'Файл';
    }
  }
  
  private async uploadFiles(files: File[], taskId: number): Promise<{success: boolean, file?: File, error?: any}[]> {
    const uploadPromises = files.map(file => 
      lastValueFrom(
        this.documentService.uploadDocument(file, taskId).pipe(
          last(),
          map(() => ({ success: true, file })),
          catchError(error => of({ success: false, file, error }))
        )
      )
    );
    
    return await Promise.all(uploadPromises);
  }
  
  openReturnDialog(task: TaskDto): void {
    const dialogRef = this.dialog.open(TaskReturnDialogComponent, {
      maxWidth: '600px',
      data: { task }
    });
  
    dialogRef.afterClosed().subscribe(comment => {
      if (comment) {
        this.returnTask(task, comment);
      }
    });
  }

  getStatusLabel(status: TaskStatus): string {
    switch(status) {
      case TaskStatus.ToDo: return 'Надо выполнить';
      case TaskStatus.Doing: return 'Выполняется';
      case TaskStatus.Checking: return 'На проверке';
      case TaskStatus.Completed: return 'Завершена';
      default: return 'Неизвестный статус';
    }
  }

  get isEditAllowed(): boolean {
    return this.data.projectStatus  === 'Designing' || this.authService.isAdmin();
  }

  ngOnInit(): void {
    setTimeout(() => {
      const list = document.querySelector('.comments-list');
      if (list) list.scrollTop = list.scrollHeight;
    }, 100);
    this.taskForm.disable();
    if (this.isEditAllowed) {
      this.initAutocomplete();
    } else {
      this.performersControl.disable();
      this.checkersControl.disable();
    }
  }

  ngAfterViewInit() {
    this.taskForm.get('performers')?.valueChanges.subscribe(() => {
      this.performersControl.updateValueAndValidity();
    });
    
    this.taskForm.get('checkers')?.valueChanges.subscribe(() => {
      this.checkersControl.updateValueAndValidity();
    });
  }

  private getExcludedUserIds(field: 'performers' | 'checkers'): number[] {
    const oppositeField = field === 'performers' ? 'checkers' : 'performers';
    return this.taskForm.get(oppositeField)?.value.map((u: UserDto) => u.id) || [];
  }

  private initAutocomplete() {
    this.filteredPerformers = this.performersControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(value => {
        const search = typeof value === 'string' ? value : '';
        const excludedIds = this.getExcludedUserIds('performers');
        
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
        const excludedIds = this.getExcludedUserIds('checkers');
        
        return search 
          ? this.userService.searchProjecters(search).pipe(
              map(users => users.filter(u => !excludedIds.includes(u.id))))
          : of([]);
      })
    );
  }

  addPerformer(user: UserDto): void {
    const performers = [...this.taskForm.value.performers, user];
    this.taskForm.patchValue({ performers });
    this.performersControl.setValue('');
  }

  addChecker(user: UserDto): void {
    const checkers = [...this.taskForm.value.checkers, user];
    this.taskForm.patchValue({ checkers });
    this.checkersControl.setValue('');
  }


  removePerformer(userId: number): void {
    const performers = this.taskForm.value.performers.filter((u: UserDto) => u.id !== userId);
    this.taskForm.patchValue({ performers });
  }

  removeChecker(userId: number): void {
    const checkers = this.taskForm.value.checkers.filter((u: UserDto) => u.id !== userId);
    this.taskForm.patchValue({ checkers });
  }

  toggleEdit(): void {
    this.isEditing = !this.isEditing;
    this.isEditing ? this.taskForm.enable() : this.taskForm.disable();
  }

  onSave(): void {
    if (!this.isEditAllowed) {
      this.snackBar.open('Сохранение изменений запрещено', 'Закрыть');
      return;
    }
    if (this.taskForm.invalid) return;
  
    this.isLoading = true;
    const updatedTask = { ...this.data.task, ...this.taskForm.value };
  
    const updateDto: PutTaskDto = {
      idTask: updatedTask.idTask,
      name: updatedTask.name,
      description: updatedTask.description,
      idPerformers: updatedTask.performers.map((u: UserDto) => u.id),
      idCheckers: updatedTask.checkers.map((u: UserDto) => u.id),
      plannedEndDate: this.taskForm.value.endDate,
      status: this.taskForm.value.status
    };
  
    this.taskService.updateTask(updateDto).subscribe({
      next: (res) => {
        this.snackBar.open('Изменения сохранены', 'Закрыть', { duration: 3000 });
        this.dialogRef.close(res); // Закрываем диалог и передаем обновленные данные
      },
      error: (err) => {
        this.snackBar.open('Ошибка сохранения', 'Закрыть');
        console.error(err);
      },
      complete: () => this.isLoading = false
    });
  }

  get sortedComments(): CommentDto[] {
    return [...this.data.task.comments].sort((a, b) => 
      new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
    );
  }

  onClose(): void {
    this.dialogRef.close();
  }

  addComment(): void {
    if (this.commentControl.invalid) return;

    const commentDto: PostCommentDto = {
      idTask: this.data.task.idTask,
      text: this.commentControl.value!
    };

    this.commentService.addComment(commentDto).subscribe({
      next: (newComment) => {
        // Добавляем новый комментарий в начало списка
        this.data.task.comments = [{
          ...newComment 
        }, ...this.data.task.comments];
        
        this.commentControl.reset();
      },
      error: (err) => {
        this.snackBar.open('Ошибка отправки комментария', 'Закрыть');
        console.error(err);
      }
    });
  }

  isOverdue(): boolean {
    if (this.data.task.status === 'Completed') return false;
    
    const now = new Date();
    const endDate = new Date(this.data.task.plannedEndDate);
    return endDate < now;
  }

  getCompletionDifference(): string {
    if (!this.data.task.actualEndDate || !this.data.task.plannedEndDate) {
      return '';
    }

    const planned = new Date(this.data.task.plannedEndDate);
    const actual = new Date(this.data.task.actualEndDate);
    const diffTime = Math.abs(actual.getTime() - planned.getTime());
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    if (actual < planned) {
      return `на ${diffDays} ${this.declension(diffDays, ['день', 'дня', 'дней'])} раньше`;
    } else {
      return `на ${diffDays} ${this.declension(diffDays, ['день', 'дня', 'дней'])} позже`;
    }
  }

  private declension(number: number, titles: [string, string, string]): string {
    const cases = [2, 0, 1, 1, 1, 2];
    return titles[
      number % 100 > 4 && number % 100 < 20
        ? 2
        : cases[number % 10 < 5 ? number % 10 : 5]
    ];
  }
}
