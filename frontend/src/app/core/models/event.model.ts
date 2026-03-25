export interface Event {
  id: string;
  name: string;
  description: string;
  eventDate: Date;
  location: string;
  createdById: string;
  createdAt: Date;
}

export interface CreateEventRequest {
  name: string;
  description: string;
  eventDate: Date;
  location: string;
}

export interface UpdateEventRequest {
  name: string;
  description: string;
  eventDate: Date;
  location: string;
}
