import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { EventService } from '../../../core/services/event.service';
import { AuthService } from '../../../core/services/auth.service';
import { Event } from '../../../core/models/event.model';
import { EventCreateDialogComponent } from '../event-create-dialog/event-create-dialog.component';


@Component({
  selector: 'app-event-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatSnackBarModule,
    MatDialogModule,
    EventCreateDialogComponent
  ],
  templateUrl: './event-list.component.html',
  styleUrl: './event-list.component.scss'
})
export class EventListComponent implements OnInit {
  private eventService = inject(EventService);
  public authService = inject(AuthService);
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);

  events: Event[] = [];
  displayedColumns: string[] = ['name', 'eventDate', 'location', 'actions'];
  loading = false;

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    this.loading = true;
    this.eventService.getEvents().subscribe({
      next: (res: Event[]) => {
        this.events = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.snackBar.open('Error al cargar eventos.', 'Cerrar', { duration: 3000 });
      }
    });
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(EventCreateDialogComponent, {
      width: '500px'
    });

    dialogRef.afterClosed().subscribe((result: any) => {
      if (result) {
        this.snackBar.open('Evento creado con éxito.', 'Cerrar', { duration: 3000 });
        this.loadEvents();
      }
    });
  }

  deleteEvent(id: string): void {

    if (confirm('¿Está seguro de eliminar este evento?')) {
      this.eventService.deleteEvent(id).subscribe({
        next: () => {
          this.snackBar.open('Evento eliminado.', 'Cerrar', { duration: 3000 });
          this.loadEvents();
        },
        error: () => this.snackBar.open('Error al eliminar evento.', 'Cerrar', { duration: 3000 })
      });
    }
  }
}
