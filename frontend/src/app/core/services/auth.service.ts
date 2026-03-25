import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { LoginResponse, User } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private readonly AUTH_TOKEN = 'eventia_token';
  private readonly USER_DATA = 'eventia_user';

  private userSignal = signal<User | null>(this.getStoredUser());
  currentUser = computed(() => this.userSignal());
  isAuthenticated = computed(() => !!this.userSignal());
  isAdmin = computed(() => this.userSignal()?.role === 'Admin');
  isSupervisor = computed(() => this.userSignal()?.role === 'Supervisor');

  constructor() { }

  login(credentials: { email: string; password: any }): Observable<LoginResponse> {
    const payload = {
      Email: credentials.email,
      Password: credentials.password
    };
    return this.http.post<LoginResponse>('http://localhost:5000/api/auth/login', payload).pipe(
      tap((res: LoginResponse) => this.handleAuthSuccess(res))
    );
  }

  // Public registration always creates an Agent (lowest privilege role)
  register(userData: { name: string; email: string; password: any }): Observable<LoginResponse> {
    const payload = {
      Name: userData.name,
      Email: userData.email,
      Password: userData.password,
      Role: 'Agent'
    };
    return this.http.post<LoginResponse>('http://localhost:5000/api/auth/register', payload).pipe(
      tap((res: LoginResponse) => this.handleAuthSuccess(res))
    );
  }

  logout(): void {
    localStorage.removeItem(this.AUTH_TOKEN);
    localStorage.removeItem(this.USER_DATA);
    this.userSignal.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.AUTH_TOKEN);
  }

  private handleAuthSuccess(res: LoginResponse): void {
    const user: User = {
      id: res.userId,
      name: res.name,
      email: res.email,
      role: res.role
    };
    localStorage.setItem(this.AUTH_TOKEN, res.token);
    localStorage.setItem(this.USER_DATA, JSON.stringify(user));
    this.userSignal.set(user);
    this.router.navigate(['/dashboard']);
  }

  private getStoredUser(): User | null {
    const data = localStorage.getItem(this.USER_DATA);
    return data ? JSON.parse(data) : null;
  }
}
