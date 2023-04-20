import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ResetPassword } from '../models/reset-password.model';

@Injectable({
  providedIn: 'root',
})
export class ResetPassService {
  private baseUrl: string = 'https://localhost:7148/api/User/';

  constructor(private http: HttpClient) {}

  sendResetPasswordLink(email: string) {
    return this.http.post<any>(`${this.baseUrl}resetemail/${email}`, {});
  }

  resetPassword(resetPass: ResetPassword) {
    return this.http.post<any>(`${this.baseUrl}resetpass`, resetPass);
  }
}
