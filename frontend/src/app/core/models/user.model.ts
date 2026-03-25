export interface User {
  id: string;
  name: string;
  email: string;
  role: string;
  isActive: boolean;
  createdAt: Date;
}

export interface CreateUserRequest {
  name: string;
  email: string;
  role: string;
}
