import { AsyncPipe, DatePipe, NgForOf, NgIf } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import {MatListModule} from '@angular/material/list';
import { PostTaskDto, PutTaskDto, TaskDetailsDto } from '../../models/task.model';
import { MatButtonModule } from '@angular/material/button';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { TaskService } from '../../services/task.service';
import {MatSnackBar} from '@angular/material/snack-bar';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { UserDto } from '../../models/user.model';
import { Observable, debounceTime, distinctUntilChanged, map, of, switchMap } from 'rxjs';
import { UserService } from '../../services/user.service';
import { CommentService } from '../../services/comment.service';
import { CommentDto, PostCommentDto } from '../../models/comment.model';
import { HasRoleDirective } from '../../directives/has-role.directive';

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
    HasRoleDirective
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

  constructor(
    private fb: FormBuilder,
    private taskService: TaskService,
    private userService: UserService,
    private commentService: CommentService,
    private snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<TaskDetailsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TaskDetailsDto
  ) {
    this.taskForm = this.fb.group({
      name: [data.name, [Validators.required, Validators.maxLength(255)]],
      description: [data.description, [Validators.required, Validators.maxLength(2000)]],
      performers: [data.performers, [Validators.required]],
      checkers: [data.checkers, [Validators.required]]
    });
  }

  ngOnInit(): void {
    setTimeout(() => {
      const list = document.querySelector('.comments-list');
      if (list) list.scrollTop = list.scrollHeight;
    }, 100);
    this.taskForm.disable();
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
    if (this.taskForm.invalid) return;
  
    this.isLoading = true;
    const updatedTask = { ...this.data, ...this.taskForm.value };
  
    const updateDto: PutTaskDto = {
      idTask: updatedTask.idTask,
      name: updatedTask.name,
      description: updatedTask.description,
      idPerformers: updatedTask.performers.map((u: UserDto) => u.id),
      idCheckers: updatedTask.checkers.map((u: UserDto) => u.id)
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
    return [...this.data.comments].sort((a, b) => 
      new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
    );
  }

  onClose(): void {
    this.dialogRef.close();
  }

  addComment(): void {
    if (this.commentControl.invalid) return;

    const commentDto: PostCommentDto = {
      idTask: this.data.idTask,
      text: this.commentControl.value!
    };

    this.commentService.addComment(commentDto).subscribe({
      next: (newComment) => {
        // Добавляем новый комментарий в начало списка
        this.data.comments = [{
          ...newComment 
        }, ...this.data.comments];
        
        this.commentControl.reset();
      },
      error: (err) => {
        this.snackBar.open('Ошибка отправки комментария', 'Закрыть');
        console.error(err);
      }
    });
  }
}
