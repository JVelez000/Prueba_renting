import { Component, Inject, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { MatChipsModule } from '@angular/material/chips';
import { TicketService } from '../../../core/services/ticket.service';
import { TicketDetail } from '../../../core/models/ticket.model';

@Component({
  selector: 'app-ticket-detail-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatListModule,
    MatChipsModule
  ],
  templateUrl: './ticket-detail-dialog.component.html',
  styleUrl: './ticket-detail-dialog.component.scss'
})
export class TicketDetailDialogComponent implements OnInit {
  private ticketService = inject(TicketService);

  detail: TicketDetail | null = null;
  loading = true;

  constructor(
    public dialogRef: MatDialogRef<TicketDetailDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { id: string }
  ) { }

  ngOnInit(): void {
    this.ticketService.getTicketById(this.data.id).subscribe({
      next: (res) => {
        this.detail = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
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
}
