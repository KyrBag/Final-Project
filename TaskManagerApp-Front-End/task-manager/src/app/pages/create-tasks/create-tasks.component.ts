import { Component, OnInit, inject } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { FormBuilder, FormGroup, ReactiveFormsModule, 
  Validators,
} from '@angular/forms';
import {  CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { ValidationError } from 'src/app/interfaces/validation-error';




@Component({
  selector: 'app-create-tasks',
  standalone: true,
  imports: [
    MatInputModule,
    ReactiveFormsModule,
    RouterLink,
    MatSelectModule,
    MatIconModule,
    MatSnackBarModule,
    CommonModule,
  ],
  templateUrl: './create-tasks.component.html',
  styleUrl: './create-tasks.component.css'
})

export class CreateTasksComponent implements OnInit {
  authService = inject(AuthService)
  fb = inject(FormBuilder);
  addTaskForm!: FormGroup;
  router = inject(Router);
  errors!: ValidationError[];
  accountDetail$ = this.authService.getDetail();
  taskForm: FormGroup;
  matSnackBar = inject(MatSnackBar);

   add() {
    if (this.addTaskForm.valid) {
      this.authService.getUsername().subscribe(username => {
        console.log('Username:', username); // Debugging line

        const taskData = this.addTaskForm.value;
        console.log("Task Data:", taskData); // Debugging line
        this.authService.addTask(taskData, username).subscribe({
          
          next: (response) => {
           console.log('Response:', response); // Debugging line
          // if (response.isSuccess) {
            // Handle success
          console.log('Task created successfully', response);
           if (response && response.message) { 
          this.matSnackBar.open(response.message, 'Close', {
            duration: 5000,
            horizontalPosition: 'center',
            verticalPosition: 'top' // Added vertical position
          })
          }else {
            console.error('Response does not contain a message property.');
          }
         console.log("navigate to account")
         this.router.navigate(['/edit']);
           
        }, 
        error: (err: HttpErrorResponse) => {
          console.error('Error:', err); // Debugging line
          // Handle error
          if (err!.status === 400) {
            this.errors = err!.error;
            this.matSnackBar.open('An error occurred. Please try again.', 'Close', {
              duration: 5000,
              horizontalPosition: 'center',
              verticalPosition: 'top' // Added vertical position
            });
          }
        },
        complete: () => console.log('Add task success'),
        
      });
    }
  )}
}


  ngOnInit(): void{
    this.addTaskForm = this.fb.group(
      {
        title: ['', Validators.required],
        description: ['', Validators.required]
      }
    )
  }

}
