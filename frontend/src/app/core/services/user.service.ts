import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private http = inject(HttpClient);
  private readonly API_URL = 'http://localhost:5000/api/users';

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.API_URL);
  }

  getUserById(id: string): Observable<User> {
    return this.http.get<User>(`${this.API_URL}/${id}`);
  }

  disableUser(id: string): Observable<void> {
    return this.http.put<void>(`${this.API_URL}/${id}/disable`, {});
  }

  enableUser(id: string): Observable<void> {
    return this.http.put<void>(`${this.API_URL}/${id}/enable`, {});
  }

  changeRole(id: string, role: string): Observable<User> {
    return this.http.put<User>(`${this.API_URL}/${id}/role`, { Role: role });
  }
}
