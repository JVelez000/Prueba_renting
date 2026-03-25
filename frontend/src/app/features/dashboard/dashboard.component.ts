import { DashboardService, DashboardStats } from '../../core/services/dashboard.service';
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MatGridListModule, MatCardModule, MatIconModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  authService = inject(AuthService);
  dashboardService = inject(DashboardService);
  currentUser = this.authService.currentUser;

  stats: any[] = [];

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    this.dashboardService.getStats().subscribe({
      next: (data: DashboardStats) => {
        this.stats = [
          { title: 'Tickets Abiertos', count: data.openTickets, icon: 'assignment', color: '#f44336' },
          { title: 'En Proceso', count: data.inProgressTickets, icon: 'autorenew', color: '#ff9800' },
          { title: 'Cerrados', count: data.closedTickets, icon: 'done_all', color: '#4caf50' },
          { title: 'Eventos Activos', count: data.activeEvents, icon: 'event', color: '#2196f3' }
        ];
      },
      error: () => {
        console.error('Error loading dashboard stats');
      }
    });
  }
}
