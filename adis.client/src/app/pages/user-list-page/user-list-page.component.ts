import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { UserDto } from '../../models/user.model';
import { UserService } from '../../services/user.service';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { DatePipe, NgClass, NgForOf } from '@angular/common';
import { UserFormComponent } from '../../components/user-form/user-form.component';
import { MatCardModule } from '@angular/material/card';
import { FormsModule } from '@angular/forms';
import { MatDivider } from '@angular/material/divider';
import { ConfirmationDialogComponent } from '../../components/confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'app-user-list-page',
  imports: [
    MatTableModule,
    MatButtonModule,
    MatChipsModule,
    MatIconModule,
    DatePipe,
    MatCardModule,
    FormsModule,
    NgClass,
    NgForOf,
    MatDivider
  ],
  templateUrl: './user-list-page.component.html',
  styleUrl: './user-list-page.component.scss'
})
export class UserListPageComponent {
  displayedColumns: string[] = ['email', 'fullName', 'role', 'createdAt'];
  users!: UserDto[];

  constructor(
    private userService: UserService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadUsers();
  }

  private loadUsers(): void {
    this.userService.getUsers().subscribe(users => {
      this.users = users;
    });
  }

  openAddUserDialog(): void {
    const dialogRef = this.dialog.open(UserFormComponent, {
      width: '500px',
      maxWidth: '500px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.userService.addUser(result).subscribe({
          next: () => this.loadUsers(),
          error: err => console.error('Ошибка добавления пользователя:', err)
        });
      }
    });
  }

  openEditUserDialog(user: UserDto): void {
    const dialogRef = this.dialog.open(UserFormComponent, {
      width: '500px',
      maxWidth: '700px',
      data: { user }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.userService.updateUser(result).subscribe({
          next: () => this.loadUsers(),
          error: err => console.error('Ошибка обновления пользователя:', err)
        });
      }
    });
  }

  deleteUser(user: UserDto): void {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: { message: `Вы уверены что хотите удалить пользователя ${user.fullName}` }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.userService.deleteUser(user.id).subscribe({
          next: () => this.loadUsers(),
          error: err => console.error('Ошибка удаления пользователя:', err)
        });
      }
    });
  }

  getRoleLabel(role: string): string {
    const rolesMap: { [key: string]: string } = {
      'admin': 'Администратор',
      'projectmanager': 'Менеджер проектов',
      'inspector': 'Инспектор',
      'projecter': 'Проектировщик'
    };

    return rolesMap[role.toLowerCase()] || rolesMap['default'];
  }
}
