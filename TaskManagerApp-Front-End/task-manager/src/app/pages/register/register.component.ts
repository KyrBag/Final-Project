import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router, RouterLink } from '@angular/router';
import { ValidationError } from 'src/app/interfaces/validation-error';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    MatInputModule,
    ReactiveFormsModule, 
    RouterLink, 
    MatIconModule,
    MatSnackBarModule,
    CommonModule,
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
 
  authService = inject(AuthService);
  matSnackBar = inject(MatSnackBar);
  fb = inject(FormBuilder);
  registerForm!: FormGroup;
  router = inject(Router);
  confirmPasswordHide: boolean =  true;
  PasswordHide: boolean = true;
  errors!: ValidationError[];

  

  register() {
    this.authService.register(this.registerForm.value).subscribe({
      next:(response) => {
        console.log(response);

        this.matSnackBar.open(response.message, 'Close', {
          duration: 5000,
          horizontalPosition: 'center',
        });
        this.router.navigate(['/login']);
      },


  error: (err: HttpErrorResponse) => {
    console.error('Error response:', err); // Log the error response

    let errorMessage = 'An unexpected error occurred.';
    if (err.status === 400) {
      if (err.error && err.error.DuplicateEmail) {
        errorMessage = err.error.DuplicateEmail[0];
      } else if (err.error instanceof Array) {
        this.errors = err.error as ValidationError[];
        errorMessage = this.errors.map(error => error.description).join(' ');
      } else {
        errorMessage = 'Validation error.';
      }

      this.matSnackBar.open(errorMessage, 'Close', {
        duration: 5000,
        verticalPosition: 'top',
        horizontalPosition: 'center',
      }).onAction().subscribe(() => {
        this.matSnackBar.dismiss();
      });
    }
  },
  complete: () => console.log('Register success'),
});
}

  ngOnInit(): void {
    this.registerForm = this.fb.group(
    {
      email: ['',[Validators.required, Validators.email]],
      password: ['',[Validators.required]],
      firstname: ['', Validators.required],
      lastname: ['', Validators.required],
      username: ['', Validators.required],
      confirmPassword:['',Validators.required],
    },
    {
      validator: this.passwordMatchValidator,
    }
  );
  
  }

  private passwordMatchValidator(control:AbstractControl):{ [key: string]: boolean } | null {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;

    if (password !== confirmPassword) {
      return { passwordMismatch: true };
    }

    return null;
  }
}
