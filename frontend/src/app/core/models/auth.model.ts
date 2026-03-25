export interface User {
  id: string;
  name: string;
  email: string;
  role: string;
}

export interface LoginResponse {
  token: string;
  name: string;
  email: string;
  role: string;
  userId: string;
}
