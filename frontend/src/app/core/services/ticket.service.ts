import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Ticket, TicketDetail, CreateTicketRequest, UpdateTicketRequest, ChangeStatusRequest } from '../models/ticket.model';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private http = inject(HttpClient);
  private readonly API_URL = 'http://localhost:5000/api/tickets';

  getTickets(): Observable<Ticket[]> {
    return this.http.get<Ticket[]>(this.API_URL);
  }

  getTicketById(id: string): Observable<TicketDetail> {
    return this.http.get<TicketDetail>(`${this.API_URL}/${id}`);
  }

  createTicket(request: CreateTicketRequest): Observable<Ticket> {
    return this.http.post<Ticket>(this.API_URL, request);
  }

  updateTicket(id: string, request: UpdateTicketRequest): Observable<Ticket> {
    return this.http.put<Ticket>(`${this.API_URL}/${id}`, request);
  }

  assignTicket(id: string, userId: string): Observable<void> {
    return this.http.put<void>(`${this.API_URL}/${id}/assign`, { userId });
  }

  changeStatus(id: string, request: ChangeStatusRequest): Observable<void> {
    return this.http.put<void>(`${this.API_URL}/${id}/status`, request);
  }

  deleteTicket(id: string): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`);
  }
}
