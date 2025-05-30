import { NgIf } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatOptionModule } from '@angular/material/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { passwordComplexityValidator } from '../../utils/validators/password.validator';
import { MatIconModule } from '@angular/material/icon';
import { UserDto } from '../../models/user.model';

@Component({
  selector: 'app-user-form',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatOptionModule,
    NgIf,
    FormsModule,
    MatSelectModule,
    MatIconModule
  ],
  templateUrl: './user-form.component.html',
  styleUrl: './user-form.component.scss'
})
export class UserFormComponent {
  userForm: FormGroup;
  showPassword = false;
  isEditMode = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<UserFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { user: UserDto }
  ) {
    this.isEditMode = !!data?.user;
    
    this.userForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [
        ...(this.isEditMode ? [] : [Validators.required]), // Пароль обязателен только при создании
        Validators.minLength(8),
        passwordComplexityValidator
      ]],
      role: ['', Validators.required],
      fullName: ['']
    });

    this.userForm.get('role')?.valueChanges.subscribe(role => {
      this.updateFullNameValidation(role);
    });

    if (this.isEditMode) {
      this.patchFormValues();
    }
  }

   private updateFullNameValidation(role: string): void {
    const fullNameControl = this.userForm.get('fullName');
    
    if (role !== 'Admin') {
      fullNameControl?.setValidators([Validators.required]);
    } else {
      fullNameControl?.clearValidators();
    }
    
    fullNameControl?.updateValueAndValidity();
  }

  private patchFormValues(): void {
    const userData = this.data.user;
    this.userForm.patchValue({
      email: userData.email,
      role: userData.role,
      fullName: userData.fullName
    });
    
    this.updateFullNameValidation(userData.role);

    // Очищаем пароль при редактировании
    this.userForm.get('password')?.setValue('');
    this.userForm.get('password')?.markAsUntouched();
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  onSubmit(): void {
    if (this.userForm.valid) {
      const result = this.isEditMode 
        ? { id: this.data.user.id, ...this.userForm.value }
        : { ...this.userForm.value };
  
      if (this.isEditMode && !result.password) {
        delete result.password;
      }
      
      this.dialogRef.close(result);
    }
  }
}
