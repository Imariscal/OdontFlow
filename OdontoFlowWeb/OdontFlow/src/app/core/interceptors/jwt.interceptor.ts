import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { MessageService } from 'primeng/api';
import { catchError, finalize, of, throwError } from 'rxjs';
import { SpinnerService } from '../services/spinner.service';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const messageService = inject(MessageService);
  const spinnerService = inject(SpinnerService);

  const token = authService.getToken();
  const isPublic = ['/auth/login', '/auth/register'].some(path => req.url.includes(path));

  const authReq = token && !isPublic
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  const excludeSpinner = ['/assets', '/config'].some(path => req.url.includes(path));
  if (!excludeSpinner) {
    spinnerService.show();
  }

  return next(authReq).pipe(
    catchError((error) => {
      if (error.status === 401 && !isPublic) {
        messageService.add({
          severity: 'warn',
          summary: 'Sesión expirada',
          detail: 'Tu sesión ha expirado. Por favor inicia sesión nuevamente.',
          life: 4000
        });
        authService.logout(); 
        return of(); 
      } 
      return throwError(() => error);
    }),
    finalize(() => spinnerService.hide())
  );
};
