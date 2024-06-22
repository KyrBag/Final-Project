import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { RegisterComponent } from './pages/register/register.component';
import { AccountComponent } from './pages/account/account.component';
import { authGuard } from './guards/auth.guard';
import { CreateTasksComponent } from './pages/create-tasks/create-tasks.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { EditTasksComponent } from './pages/edit-tasks/edit-tasks.component';
import { UpdateTaskComponent } from './pages/update-task/update-task.component';
import { AccountSettingsComponent } from './pages/account-settings/account-settings.component';

export const routes: Routes = [
    {
        path: '',
        component: HomeComponent
    },
    {
        path: 'login', 
        component: LoginComponent
    },
    {
        path: 'register', 
        component: RegisterComponent,
    },
    {
        path: 'account/:id', 
        component: AccountComponent,
        canActivate: [authGuard],
    },
    {
        path: 'create', 
        component: CreateTasksComponent,
        canActivate: [authGuard],
    },
    {
        path: 'update/:id', 
        component: UpdateTaskComponent,
        canActivate: [authGuard],
    },
    {
        path: 'edit', 
        component: EditTasksComponent,
        canActivate: [authGuard],
    },
    {
        path: 'profile/:id', 
        component: AccountSettingsComponent,
        canActivate: [authGuard],
    },
    
];
