import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core'; 
import { AuthService } from '../services/auth.service';

export const roleGuard = (allowedRoles: string[]): CanActivateFn => () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const isValid = auth.isAuthenticated() && !auth.isTokenExpired() && auth.hasAnyRole(allowedRoles);

  if (!isValid) {
    router.navigate(['/notfound']); // o '/unauthorized'
    return false;
  }

  return true;
};
