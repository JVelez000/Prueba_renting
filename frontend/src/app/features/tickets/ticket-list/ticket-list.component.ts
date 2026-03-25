import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatMenuModule } from '@angular/material/menu';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { TicketService } from '../../../core/services/ticket.service';
import { AuthService } from '../../../core/services/auth.service';
import { Ticket } from '../../../core/models/ticket.model';
import { TicketDetailDialogComponent } from '../ticket-detail-dialog/ticket-detail-dialog.component';
import { TicketCreateDialogComponent } from '../ticket-create-dialog/ticket-create-dialog.component';


@Component({
  selector: 'app-ticket-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatMenuModule,
    MatSnackBarModule,
    MatDialogModule,
    TicketDetailDialogComponent
  ],
  templateUrl: './ticket-list.component.html',
  styleUrl: './ticket-list.component.scss'
})
export class TicketListComponent implements OnInit {
  private ticketService = inject(TicketService);
  public authService = inject(AuthService);
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);

  tickets: Ticket[] = [];
  displayedColumns: string[] = ['id', 'title', 'event', 'status', 'assignedTo', 'actions'];
  loading = false;

  ngOnInit(): void {
    this.loadTickets();
  }

  loadTickets(): void {
    this.loading = true;
    this.ticketService.getTickets().subscribe({
      next: (res: Ticket[]) => {
        this.tickets = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.snackBar.open('Error al cargar tickets.', 'Cerrar', { duration: 3000 });
      }
    });
  }

  getStatusColor(status: string): string {
    switch (status.toLowerCase()) {
      case 'open': return 'primary';
      case 'inprogress': return 'accent';
      case 'closed': return 'warn';
      default: return '';
    }
  }

  openDetail(id: string): void {
    this.dialog.open(TicketDetailDialogComponent, {
      width: '600px',
      data: { id }
    });
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(TicketCreateDialogComponent, {
      width: '500px'
    });

    dialogRef.afterClosed().subscribe((result: any) => {
      if (result) {
        this.snackBar.open('Ticket creado con éxito.', 'Cerrar', { duration: 3000 });
        this.loadTickets();
      }
    });
  }

  deleteTicket(id: string): void {

    if (confirm('¿Está seguro de eliminar este ticket?')) {
      this.ticketService.deleteTicket(id).subscribe({
        next: () => {
          this.snackBar.open('Ticket eliminado.', 'Cerrar', { duration: 3000 });
          this.loadTickets();
        },
        error: () => this.snackBar.open('Error al eliminar ticket.', 'Cerrar', { duration: 3000 })
      });
    }
  }
}
