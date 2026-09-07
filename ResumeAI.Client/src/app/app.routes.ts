import { Routes } from '@angular/router';

import { RegisterComponent } from './register/register';
import { LoginComponent } from './login/login';
import { ResumeAnalyzerComponent } from './resume-analyzer/resume-analyzer';
import { authGuards } from './guards/auth-guard';

export const routes: Routes = [

  // Application start
  {
    path: '',
    redirectTo: 'register',
    pathMatch: 'full'
  },

  // Registration
  {
    path: 'register',
    component: RegisterComponent
  },

  // Login
  {
    path: 'login',
    component: LoginComponent
  },

  // Resume Analyzer
  {
    path: 'resume-analyzer',
    component: ResumeAnalyzerComponent,
     canActivate: [authGuards]
  },

  // Any invalid URL
  {
    path: '**',
    redirectTo: 'register'
  }

];