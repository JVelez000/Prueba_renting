import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { TicketService } from '../../../core/services/ticket.service';
import { EventService } from '../../../core/services/event.service';
import { Event } from '../../../core/models/event.model';
import { Ticket, CreateTicketRequest } from '../../../core/models/ticket.model';

@Component({
  selector: 'app-ticket-create-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule
  ],
  templateUrl: './ticket-create-dialog.component.html',
  styles: [`
    mat-form-field { width: 100%; margin-bottom: 10px; }
    .actions { display: flex; justify-content: flex-end; gap: 10px; margin-top: 20px; }
  `]
})
export class TicketCreateDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private ticketService = inject(TicketService);
  private eventService = inject(EventService);
  private dialogRef = inject(MatDialogRef<TicketCreateDialogComponent>);

  ticketForm: FormGroup = this.fb.group({
    title: ['', [Validators.required, Validators.minLength(5)]],
    description: ['', [Validators.required]],
    eventId: ['', [Validators.required]]
  });

  events: Event[] = [];
  loadingEvents = false;
  submitting = false;

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    this.loadingEvents = true;
    this.eventService.getEvents().subscribe({
      next: (res: Event[]) => {
        this.events = res;
        this.loadingEvents = false;
      },
      error: () => {
        this.loadingEvents = false;
        console.error('Error loading events for ticket creation');
      }
    });
  }

  onSubmit(): void {
    if (this.ticketForm.valid) {
      this.submitting = true;
      const request: CreateTicketRequest = {
        title: this.ticketForm.value.title,
        description: this.ticketForm.value.description,
        eventId: this.ticketForm.value.eventId
      };

      this.ticketService.createTicket(request).subscribe({
        next: (res: Ticket) => {
          this.submitting = false;
          this.dialogRef.close(res);
        },
        error: (err: Error) => {
          this.submitting = false;
          console.error('Error creating ticket', err);
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
