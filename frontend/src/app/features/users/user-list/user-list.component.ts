import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { UserService } from '../../../core/services/user.service';
import { AuthService } from '../../../core/services/auth.service';
import { User } from '../../../core/models/user.model';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatSelectModule,
    MatOptionModule,
    MatFormFieldModule,
    MatSnackBarModule,
    MatTooltipModule
  ],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss'
})
export class UserListComponent implements OnInit {
  private userService = inject(UserService);
  public authService = inject(AuthService);
  private snackBar = inject(MatSnackBar);

  users: User[] = [];
  displayedColumns: string[] = ['name', 'email', 'role', 'status', 'actions'];
  loading = false;
  availableRoles = ['Agent', 'Supervisor', 'Admin'];

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading = true;
    this.userService.getUsers().subscribe({
      next: (res) => {
        this.users = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.snackBar.open('Error al cargar usuarios.', 'Cerrar', { duration: 3000 });
      }
    });
  }

  toggleUserStatus(user: User): void {
    const action = user.isActive ? this.userService.disableUser(user.id) : this.userService.enableUser(user.id);
    action.subscribe({
      next: () => {
        this.snackBar.open(`Usuario ${user.isActive ? 'deshabilitado' : 'habilitado'}.`, 'Cerrar', { duration: 3000 });
        this.loadUsers();
      },
      error: () => this.snackBar.open('Error al cambiar estado.', 'Cerrar', { duration: 3000 })
    });
  }

  onRoleChange(user: User, newRole: string): void {
    this.userService.changeRole(user.id, newRole).subscribe({
      next: () => {
        this.snackBar.open(`Rol de ${user.name} cambiado a ${newRole}.`, 'Cerrar', { duration: 3000 });
        this.loadUsers();
      },
      error: () => this.snackBar.open('Error al cambiar rol.', 'Cerrar', { duration: 3000 })
    });
  }

  getRoleColor(role: string): string {
    switch (role) {
      case 'Admin': return 'warn';
      case 'Supervisor': return 'accent';
      default: return 'primary';
    }
  }
}
