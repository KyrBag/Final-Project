import { MatSnackBar } from '@angular/material/snack-bar';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  const matSnackBar = inject(MatSnackBar);

  if (inject(AuthService).isLoggedIn()) {
    return true;
  }

  matSnackBar.open('You must Login to view this page', 'Ok',{
    duration: 3000
  });
  inject(Router).navigate(['/']);
   return false;
};
