import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  
  private apiUrl = 'https://resumeai-h94i.onrender.com/api/Auth';

  constructor(private http: HttpClient) {}

  register(
    name: string,
    email: string,
    password: string
  ): Observable<any> {

    const user = {
      name: name,
      email: email,
      password: password
    };

    return this.http.post(
      `${this.apiUrl}/register`,
      user
    );
  }

  login(
    email: string,
    password: string
  ): Observable<any> {

    const user = {
      email: email,
      password: password
    };

    return this.http.post(
      `${this.apiUrl}/login`,
      user
    );
  }
}