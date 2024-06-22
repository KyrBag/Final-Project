import { Injectable, effect, signal } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { LoginRequest } from '../interfaces/login-request';
import { Observable, map, of } from 'rxjs';
import { AuthResponse } from '../interfaces/auth-response';
import { HttpClient } from '@angular/common/http';
import { jwtDecode } from 'jwt-decode';
import { RegisterRequest } from '../interfaces/register-request';
import { UserDetail } from '../interfaces/user-detail';
import { CreateTaskRequest } from '../interfaces/create-task-request';
import { UserTasks } from '../interfaces/user-tasks';
import { EditTaskRequest } from '../interfaces/edit-task-request';
import { UpdateUserProfile } from '../interfaces/profile-update-details';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  
  apiUrl:string = environment.apiUrl;
  private tokenKey='token'
  

   constructor(private http:HttpClient) {}

  login(data: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}api/Account/login`, data)
      .pipe(
        map((response) => {
          if (response.isSuccess) {
            localStorage.setItem(this.tokenKey, response.token);
          }
          return response;
        })
      );
  }

  register(data: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}api/Account/register`, data);
  }

  updateProfile(id: string, profile: UpdateUserProfile): Observable<any> {
    return this.http.put(`${this.apiUrl}api/Account/update/${id}`, profile);
  }

  deleteUser(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}api/Account/delete/${id}`);
  }

  getDetail=():Observable<UserDetail> =>
    this.http.get<UserDetail>(`${this.apiUrl}api/Account/me`);
  
  addTask(task: Partial<CreateTaskRequest>, username: string): Observable<AuthResponse> {
    const payload: CreateTaskRequest = {
      title: task.title!,
      description: task.description!,
      username: username,
      dueDate: new Date() // You can adjust this if you want to take it from the user
    };
    console.log('Payload:', payload); // Debugging line
    return this.http.post<AuthResponse>(`${this.apiUrl}api/TaskItems/CreateTaskItem`, payload).pipe(
      map((response) => {
        if (response.isSuccess!) {
          localStorage.setItem('tokenKey', response.token);
        }
        console.log("2")
        return response;
      })
    );
  }

  deleteTask(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}api/TaskItems/DeleteTaskItem/${id}`);
  }

  updateTask(taskId: string, taskData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}api/TaskItems/UpdateTaskItem/${taskId}`, taskData); // Ensure taskId is just the ID, without any prefix like ':id'
  }
  
  getTaskById(id: string): Observable<any> {
      return this.http.get(`${this.apiUrl}api/TaskItems/GetTaskItemById/${id}`);
     }

   
  getTasks(): Observable<UserTasks[]> {
   return this.http.get<UserDetail>(`${this.apiUrl}api/Account/me`).pipe(
    map(response => response.tasks)
  );
  }

  getUsername(): Observable<string> {
      return this.http.get<UserDetail>(`${this.apiUrl}api/Account/me`).pipe(
        map(user => user.username) 
      );
    }

 



  getUserDetail=()=>{
    const token = this.getToken();
    if(!token) return null;
    const decodedToken:any = jwtDecode(token);
    const userDetail = {
      id:decodedToken.sub,
      fullName:decodedToken.unique_name,
      email:decodedToken.email,
      roles:decodedToken.role || []
    }

    return userDetail;
  }

  isLoggedIn=():boolean=>{
    const token = this.getToken();
    if(!token) return false;
    return !this.isTokenExpired();
  };

  private isTokenExpired() {
     const token = this.getToken();
     if(!token) return true;
     const decoded = jwtDecode(token);
     const isTokenExpired = Date.now() >= decoded['exp']! * 1000;
     if(isTokenExpired) this.logout();
     return isTokenExpired;
  }

  logout = (): void => {
    localStorage.removeItem(this.tokenKey)
  };

   getToken = ():string|null => localStorage.getItem(this.tokenKey) || '';
}
