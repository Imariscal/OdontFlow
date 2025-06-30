import { Routes } from '@angular/router';
import { Access } from './access'; 
import { Error } from './error';
import { Login } from './login/login';
import { ChangePassword } from './change-password/change-password';
import { LogOut } from './log-out/logOut';

export default [
    { path: 'access', component: Access },
    { path: 'error', component: Error },
    { path: 'login', component: Login },
    { path: 'changePassword', component: ChangePassword },
    { path: 'logout', component: LogOut }
] as Routes;
