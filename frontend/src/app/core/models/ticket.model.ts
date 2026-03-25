export interface Ticket {
  id: string;
  title: string;
  description: string;
  status: string;
  eventId: string;
  eventName?: string;
  createdById: string;
  createdByName?: string;
  assignedToId?: string;
  assignedToName?: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface TicketHistory {
  id: string;
  oldStatus: string;
  newStatus: string;
  changedBy: string;
  notes?: string;
  createdAt: Date;
}

export interface TicketDetail {
  ticket: Ticket;
  history: TicketHistory[];
}

export interface CreateTicketRequest {
  title: string;
  description: string;
  eventId: string;
}

export interface UpdateTicketRequest {
  title: string;
  description: string;
}

export interface ChangeStatusRequest {
  status: string;
  notes?: string;
}
