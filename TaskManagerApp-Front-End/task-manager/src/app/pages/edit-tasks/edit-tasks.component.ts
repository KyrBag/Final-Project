import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild, inject, ChangeDetectorRef } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { MatPaginatorModule } from '@angular/material/paginator';
import { Router, RouterLink } from '@angular/router';
import { UserTasks } from 'src/app/interfaces/user-tasks';
import { AuthService } from 'src/app/services/auth.service';
import { Observable } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ToastrService } from 'ngx-toastr';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import {MatCardModule} from '@angular/material/card';




@Component({
  selector: 'app-edit-tasks',
  standalone: true,
  imports: [
    MatCardModule,
    RouterLink, 
    MatTableModule,
    MatButtonModule,
    MatIconModule, 
    CommonModule,
    // MatSortModule,
    MatPaginatorModule,
  ],
  templateUrl: './edit-tasks.component.html',
  styleUrl: './edit-tasks.component.css'
})

export class EditTasksComponent implements OnInit {
  
  toaster = inject(ToastrService)
  matSnackBar = inject(MatSnackBar)
  router = inject(Router);
  displayedColumns: string[] = ['id', 'title', 'description', 'dueDate', 'isCompleted', 'action'];
  taskDetail$: Observable<UserTasks[]>;
  dataSource: MatTableDataSource<UserTasks> = new MatTableDataSource<UserTasks>();


  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;

  

  constructor(private authService: AuthService, private cdr: ChangeDetectorRef) { }
  
 

  ngAfterViewInit(): void {
    this.setPaginatorAndSort();
    
  }

  ngOnInit(): void {
    this.loadTasks();
    
  }

  // delete(taskId: number) {
  //   if (!confirm("Are you sure you want to delete your task? This action cannot be undone.")) {
    
  //   this.authService.deleteTask(taskId).subscribe(() => {
     
  //     // Refresh the task list or handle the UI update
  //     this.taskDetail$ = this.authService.getTasks();
  //     this.matSnackBar.open("Task was deleted successfully.", 'Close', { 
          
  //       duration: 5000,
  //       horizontalPosition: 'center',
  //       verticalPosition: 'top' // Added vertical position
  //     })
      
  //     console.log("task deleted with id" + taskId)
    
  //   });
  // }
  // }

  delete(taskId: number) {
    if (confirm("Are you sure you want to delete your task? This action cannot be undone.")) {
      this.authService.deleteTask(taskId).subscribe(() => {
        // Refresh the task list or handle the UI update
        this.taskDetail$ = this.authService.getTasks();
        this.matSnackBar.open("Task was deleted successfully.", 'Close', {
          duration: 5000,
          horizontalPosition: 'center',
          verticalPosition: 'top' // Added vertical position
        });
         // Refresh the page to show updated task list
        window.location.reload();
        console.log("task deleted with id " + taskId);
      }, error => {
        console.error("Error deleting task:", error);
        this.matSnackBar.open("Failed to delete the task. Please try again.", 'Close', {
          duration: 5000,
          horizontalPosition: 'center',
          verticalPosition: 'top'
        });
      });
    }
  }

  edit(id:string) {
    console.log(id);
    this.router.navigate(['/update', id])
  }

  markAsCompleted(task: any): void {
    task.isCompleted = true;
    this.authService.updateTask(task.id, task).subscribe(
      () => {
        this.toaster.success("Success")
        this.matSnackBar.open("Congratulations! Task successfully completed.Keep up the good work.", 'Close', { 
          duration: 5000,
          horizontalPosition: 'center',
          verticalPosition: 'top' // Added vertical position
        })
        console.log('Task marked as completed.');
        this.loadTasks(); // Refresh task list after update
      },
      error => {
        console.error('Error updating task:', error);
      }
    );
  }

  markAsNotCompleted(task: any): void {
    task.isCompleted = false;
    this.authService.updateTask(task.id, task).subscribe(
      () => {
        this.matSnackBar.open("On the way! Task status: ongoing.", 'Close', { 
          duration: 5000,
          horizontalPosition: 'center',
          verticalPosition: 'top' // Added vertical position
        })
        console.log('Task marked as not completed.');
        this.loadTasks(); // Refresh task list after update
      },
      error => {
        console.error('Error updating task:', error);
      }
    );
  }


  setPaginatorAndSort(): void {
    if (this.dataSource) {
      // this.dataSource.sort = this.sort;
      this.dataSource.paginator = this.paginator;
    }
  }

  loadTasks(): void {
    this.taskDetail$ = this.authService.getTasks();
    this.taskDetail$.subscribe(data => {
      this.dataSource.data = data;
      this.setPaginatorAndSort();
      this.cdr.detectChanges(); // Ensure the view updates
      // this.dataSource.data = tasks;
      // this.dataSource.sort = this.sort; // Ensure the sort is set after data is loaded
      // this.dataSource.paginator = this.paginator; // Ensure the paginator is set after data is loaded
      // this.cdr.detectChanges(); // Trigger change detection
      console.log('Task Detail:', data);
    });
   
  }
}

