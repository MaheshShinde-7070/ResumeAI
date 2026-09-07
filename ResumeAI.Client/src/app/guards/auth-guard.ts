import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

export const authGuards: CanActivateFn = (route, state) => {

  const token = localStorage.getItem('token');

  if (token) {
    return true;
  }

  const router = inject(Router);

  return router.createUrlTree(['/login']);
};