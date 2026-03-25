import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Event, CreateEventRequest, UpdateEventRequest } from '../models/event.model';

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private http = inject(HttpClient);
  private readonly API_URL = 'http://localhost:5000/api/events';

  getEvents(): Observable<Event[]> {
    return this.http.get<Event[]>(this.API_URL);
  }

  getEventById(id: string): Observable<Event> {
    return this.http.get<Event>(`${this.API_URL}/${id}`);
  }

  createEvent(request: CreateEventRequest): Observable<Event> {
    return this.http.post<Event>(this.API_URL, request);
  }

  updateEvent(id: string, request: UpdateEventRequest): Observable<Event> {
    return this.http.put<Event>(`${this.API_URL}/${id}`, request);
  }

  deleteEvent(id: string): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`);
  }
}
