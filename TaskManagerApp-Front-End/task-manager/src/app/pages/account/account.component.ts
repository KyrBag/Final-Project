import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, OnInit, ViewChild, inject, } from '@angular/core';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { AuthService } from 'src/app/services/auth.service';
import { MatCardModule } from '@angular/material/card';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { Observable } from 'rxjs';
import { UserTasks } from 'src/app/interfaces/user-tasks';

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [
    CommonModule, 
    MatListModule, 
    MatDividerModule, 
    MatIconModule, 
    MatCardModule, 
    MatTableModule,
    MatPaginatorModule,
  ],
  templateUrl: './account.component.html',
  styleUrl: './account.component.css'
})
export class AccountComponent implements OnInit {
  authService = inject(AuthService);
  accountDetail$ = this.authService.getDetail();
  taskDetail$: Observable<UserTasks[]>;
  dataSource: MatTableDataSource<UserTasks> = new MatTableDataSource<UserTasks>();
  displayedColumns: string[] = ['id', 'title', 'description', 'dueDate', 'isCompleted'];

  // @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  
  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  ngOnInit(): void {
    this.loadTasks();
    // this.dataSource.sort = this.sort;
  }

  constructor() {
    this.accountDetail$.subscribe(data => console.log('User Detail:', data));
    const User = this.authService.getTasks();
    // this.dataSource = new MatTableDataSource(tasks = UserTasks);
  }

  loadTasks(): void {
    this.taskDetail$ = this.authService.getTasks();
    this.taskDetail$.subscribe(data => {
      this.dataSource.data = data;
      // Ensure the view updates
      // this.dataSource.data = tasks;
      // this.dataSource.sort = this.sort; // Ensure the sort is set after data is loaded
      this.dataSource.paginator = this.paginator; // Ensure the paginator is set after data is loaded
      // this.cdr.detectChanges(); // Trigger change detection
      console.log('Task Detail:', data);
    });
}
}
