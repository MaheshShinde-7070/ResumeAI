import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ResumeService {

  private apiUrl = 'https://resumeai-h94i.onrender.com/api/Resume';

  constructor(private http: HttpClient) {}

  uploadResume(file: File): Observable<any> {

    const formData = new FormData();

    formData.append('file', file);

    return this.http.post(
      `${this.apiUrl}/upload`,
      formData
    );
  }
  analyzeJob(jobDescription: string): Observable<any> {
  return this.http.post(
    `${this.apiUrl}/analyze-job`,
    {
      jobDescription: jobDescription
    }
  );
}
}