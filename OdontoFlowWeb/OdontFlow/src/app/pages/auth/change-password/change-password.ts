import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators,  AbstractControl, ValidationErrors } from '@angular/forms';
import { AppFloatingConfigurator } from '../../../layout/component/app.floatingconfigurator';
import { PrimengModule } from '../../../shared/primeng.module'; 
import { AuthService } from '../../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NotificationService } from '../../../core/services/notification.service';
import { ToastModule } from 'primeng/toast';


@Component({ 
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, PrimengModule,ToastModule,  AppFloatingConfigurator],
  templateUrl: './change-password.html'
})
export class ChangePassword {
  fb = inject(FormBuilder);
  authService = inject(AuthService);
  router = inject(Router);
  notificationService = inject(NotificationService)
  loginError = false;

  form: FormGroup = this.fb.group({
    password: ['', [
      Validators.required,
      Validators.minLength(8),
      Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*[\$%&!@]).{8,}$/)
    ]],
    confirmPassword: ['', Validators.required],
    remember: [false]
  }, {
    validators: this.passwordsMatchValidator
  });

onSubmit(): void {
  this.loginError = false;

  if (this.form.valid) {
    const { password } = this.form.value;
    this.authService.resetPassword(password).subscribe({
      next: (res: any) => {     

        localStorage.setItem('token', res.payload.token);
        this.authService.setToken(res.payload.token);
        this.router.navigate(['/']);
      },
      error: () => {
        this.loginError = true; 
      }
    });
  } else {
    this.form.markAllAsTouched();
  }
}

passwordsMatchValidator(group: AbstractControl): ValidationErrors | null {
  const password = group.get('password')?.value;
  const confirm = group.get('confirmPassword')?.value;
  return password === confirm ? null : { passwordsMismatch: true };
}

  
}
