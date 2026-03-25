import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { EventService } from '../../../core/services/event.service';
import { Event, CreateEventRequest } from '../../../core/models/event.model';

@Component({
  selector: 'app-event-create-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule
  ],
  templateUrl: './event-create-dialog.component.html',
  styles: [`
    mat-form-field { width: 100%; margin-bottom: 10px; }
    .actions { display: flex; justify-content: flex-end; gap: 10px; margin-top: 20px; }
  `]
})
export class EventCreateDialogComponent {
  private fb = inject(FormBuilder);
  private eventService = inject(EventService);
  private dialogRef = inject(MatDialogRef<EventCreateDialogComponent>);

  eventForm: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    description: ['', [Validators.required]],
    eventDate: [new Date(), [Validators.required]],
    location: ['', [Validators.required]]
  });

  submitting = false;

  onSubmit(): void {
    if (this.eventForm.valid) {
      this.submitting = true;
      const request: CreateEventRequest = {
        name: this.eventForm.value.name,
        description: this.eventForm.value.description,
        eventDate: this.eventForm.value.eventDate,
        location: this.eventForm.value.location
      };

      this.eventService.createEvent(request).subscribe({
        next: (res: Event) => {
          this.submitting = false;
          this.dialogRef.close(res);
        },
        error: (err: Error) => {
          this.submitting = false;
          console.error('Error creating event', err);
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
