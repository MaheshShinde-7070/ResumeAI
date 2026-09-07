import { Component,ChangeDetectorRef } from '@angular/core';
import { ResumeService } from '../services/resume';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
@Component({
  selector: 'app-resume-analyzer',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './resume-analyzer.html',
  styleUrl: './resume-analyzer.css'
})
export class ResumeAnalyzerComponent {

  selectedFile: File | null = null;

  jobDescription = '';

  jobMatch: any = null;

  isJobAnalyzing = false;

  isUploading = false;

  errorMessage = '';

  constructor(private resumeService: ResumeService,private cdr: ChangeDetectorRef,private router: Router) {}

  onFileSelected(event: Event) {

    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      this.errorMessage = '';
    }
  }

  uploadResume() {

    if (!this.selectedFile) {
      this.errorMessage = 'Please select a PDF resume.';
      return;
    }

    this.isUploading = true;
    
    this.errorMessage = '';

    this.resumeService.uploadResume(this.selectedFile)
      .subscribe({
  next: (response) => {

  console.log('1. RESPONSE RECEIVED');

  console.log('2. ANALYSIS SET');

  this.isUploading = false;

  this.cdr.detectChanges();

  console.log('3. UPLOADING FALSE');
},

        error: (error) => {

          console.error(error);

          this.errorMessage = 'Failed to analyze resume.';

          this.isUploading = false;
        }
      });
  }

  analyzeJob() {

    if (!this.selectedFile) {
  this.errorMessage = 'Please upload your resume first.';
  return;
  }

  if (!this.jobDescription.trim()) {
    this.errorMessage = 'Please enter a job description.';
    return;
  }

  this.isJobAnalyzing = true;
  this.errorMessage = '';

  this.resumeService
    .analyzeJob(this.jobDescription)
    .subscribe({
      next: (response) => {

        console.log('JOB MATCH RESPONSE:', response);

        this.jobMatch = response;

        this.isJobAnalyzing = false;

        this.cdr.detectChanges();
      },

      error: (error) => {

        console.error(error);

        this.errorMessage = 'Failed to analyze job match.';

        this.isJobAnalyzing = false;

        this.cdr.detectChanges();
      }
    });
}

  logout() {
  localStorage.removeItem('token');

  this.router.navigate(['/login']);
}
}