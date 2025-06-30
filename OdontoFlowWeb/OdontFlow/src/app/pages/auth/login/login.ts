import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AppFloatingConfigurator } from '../../../layout/component/app.floatingconfigurator';
import { PrimengModule } from '../../../shared/primeng.module'; 
import { AuthService } from '../../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NotificationService } from '../../../core/services/notification.service';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, PrimengModule,ToastModule,  AppFloatingConfigurator],
  templateUrl: './login.html'
})
export class Login {
  fb = inject(FormBuilder);
  authService = inject(AuthService);
  router = inject(Router);
  notificationService = inject(NotificationService)
  loginError = false;

  form: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
    remember: [false]
  });

onSubmit(): void {
  this.loginError = false;

  if (this.form.valid) {
    const { email, password } = this.form.value;
    this.authService.login(email, password).subscribe({
      next: (res: any) => {
        localStorage.setItem('token', res.payload.token);
        this.authService.setToken(res.payload.token);
        if (res.payload.changePassword) {
          this.router.navigate(['/auth/changePassword']); 
        }
        else {        
          this.router.navigate(['/']);
        }
      },
      error: () => {
        this.loginError = true; 
      }
    });
  } else {
    this.form.markAllAsTouched();
  }
}

  
}
