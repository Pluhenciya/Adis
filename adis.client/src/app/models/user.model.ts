export interface UserDto {
    id: number;
    email: string;
    password: string;
    role: string;
    fullName?: string;
    createdAt: Date;
  }