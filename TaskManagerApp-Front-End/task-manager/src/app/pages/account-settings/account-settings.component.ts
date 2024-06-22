import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router, RouterLink } from '@angular/router';
import { UpdateUserProfile } from 'src/app/interfaces/profile-update-details';
import { UserDetail } from 'src/app/interfaces/user-detail';
import { ValidationError } from 'src/app/interfaces/validation-error';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-account-settings',
  standalone: true,
  imports: [
    MatInputModule,
    ReactiveFormsModule,
    RouterLink,
    MatIconModule,
    MatSnackBarModule,
    CommonModule,
  ],
  templateUrl: './account-settings.component.html',
  styleUrl: './account-settings.component.css'
})
export class AccountSettingsComponent {

  authService = inject(AuthService);
  router = inject(Router);
  fb = inject(FormBuilder);
  errors!: ValidationError[];
  accountDetail$ = this.authService.getDetail();
  profileForm: FormGroup;
  matSnackBar = inject(MatSnackBar);

  profile: UpdateUserProfile = {

    username: '',
    email: '',
    firstName: '',
    lastName: '',
  };

  userId: string = '';
   
  ngOnInit(): void {
    this.profileForm = this.fb.group({
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      firstname: ['', Validators.required],
      lastname: ['', Validators.required] 
    });
    this.loadUserProfile();
  }

  loadUserProfile(): void {
    this.authService.getDetail().subscribe({
      next: (profile: UserDetail) => {
        this.userId = profile.userId;
        this.profileForm.patchValue({
          username: profile.username,
          email: profile.email,
          firstname: profile.firstName,
          lastname: profile.lastName
        });
      },
      error: (error) => {
        console.error('Error loading user profile:', error);
      }
    });
  }

  updateProfile(): void {
    if (this.profileForm.invalid) {
      return;
    }

    const updatedProfile: UpdateUserProfile = this.profileForm.value;

    this.authService.updateProfile(this.userId, updatedProfile).subscribe({
      next: () => {
        this.matSnackBar.open("Profile updated successfully.", 'Close', {
          duration: 5000,
          horizontalPosition: 'center',
          verticalPosition: 'top'
        });
        this.router.navigate(['/']);
      },
      error: (error) => {
        console.error('Error updating profile:', error);
        this.matSnackBar.open("Error updating profile. Please try again.", 'Close', {
          duration: 5000,
          horizontalPosition: 'center',
          verticalPosition: 'top'
        });
      }
    });
  }


deleteUser(): void {
  if (!confirm("Are you sure you want to delete your account? This action cannot be undone.")) {
    return;
  }

  this.authService.deleteUser(this.userId).subscribe({
    next: () => {
      this.matSnackBar.open("Account deleted successfully.", 'Close', {
        duration: 5000,
        horizontalPosition: 'center',
        verticalPosition: 'top'
      });
      this.authService.logout(); // Log the user out
      this.router.navigate(['/login']); // Redirect to login or home page after deletion
    },
    error: (error) => {
      console.error('Error deleting account:', error);
      this.matSnackBar.open("Error deleting account. Please try again.", 'Close', {
        duration: 5000,
        horizontalPosition: 'center',
        verticalPosition: 'top'
      });
    }
  });
}
}

