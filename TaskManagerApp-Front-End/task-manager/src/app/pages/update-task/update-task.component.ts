import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { FormBuilder, FormGroup, ReactiveFormsModule, 
  Validators,
} from '@angular/forms';
import {  CommonModule } from '@angular/common';
import { ValidationError } from 'src/app/interfaces/validation-error';
import { HttpErrorResponse } from '@angular/common/http';
import { EditTaskRequest } from 'src/app/interfaces/edit-task-request';
import { ToastrService } from 'ngx-toastr';



@Component({
  selector: 'app-update-task',
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
  templateUrl: './update-task.component.html',
  styleUrl: './update-task.component.css'
})
export class UpdateTaskComponent implements OnInit {
    authSerivce = inject(AuthService)
    router = inject(Router);
    route = inject(ActivatedRoute)
    authService = inject(AuthService)
    fb = inject(FormBuilder);
    updateTaskForm!: FormGroup;
    errors!: ValidationError[];
    accountDetail$ = this.authService.getDetail();
    taskForm: FormGroup;
    matSnackBar = inject(MatSnackBar);
    toaster = inject(ToastrService)

    task: EditTaskRequest = {
      id: '',
      title: '',
      description: '',
      dueDate: new Date(),
      isCompleted: false,
      username: ''
    };

    ngOnInit(): void {
      this.updateTaskForm = this.fb.group({
        id: [''], // Ensure id is included in the form
        title: ['', Validators.required],
        description: ['', Validators.required],
        dueDate: [''],
        isCompleted: [''],
        username: [''] // Assuming you have a way to fetch the username or set it dynamically
      });
  
      this.route.paramMap.subscribe(params => {
        const taskId = params.get('id');
        if (taskId) {
          this.authService.getTaskById(taskId).subscribe(
            (task: any) => { // Replace 'any' with your specific Task interface/type
              this.task = task;
              this.updateTaskForm.patchValue({
                id: task.id,
                title: task.title,
                description: task.description,
                dueDate: task.dueDate,
                isCompleted: task.isCompleted,
                username: 'current_username' // Replace with the actual username or fetch dynamically
              });
            },
            error => {
              console.error('Error fetching task:', error);
            }
          );
        } else {
          console.error('No task ID provided in route parameters.');
        }
      });
    }

    
     onSubmit(): void {
      const editTaskRequest: EditTaskRequest = {
        id: this.updateTaskForm.value.id,
        title: this.updateTaskForm.value.title,
        description: this.updateTaskForm.value.description,
        dueDate: this.updateTaskForm.value.dueDate,
        isCompleted: this.updateTaskForm.value.isCompleted,
        username: this.updateTaskForm.value.username
      };
  
      this.authService.updateTask(editTaskRequest.id, editTaskRequest).subscribe(
        () => {
          this.router.navigate(['/edit'])
          this.matSnackBar.open("Success!  Task was updated.", 'Close', { 
          
            duration: 5000,
            horizontalPosition: 'center',
            verticalPosition: 'top' // Added vertical position
          })
          console.log('Task updated successfully.');
          this.toaster.success("Update Success ! ")
          // Optionally, navigate to a success page or handle success case
        },
        error => {
          console.error('Error updating task:', error);
          // Handle error appropriately, e.g., show an error message to the user
        }
      );
    }

}
