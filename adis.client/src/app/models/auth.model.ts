export interface LoginRequest {
    email: string;
    password: string;
  }
  
  export interface RefreshTokenRequest {
    accessToken: string;
    refreshToken: string;
  }
  
  export interface AuthResponse {
    accessToken: string;
    expiresIn: number;
    tokenType: string;
    refreshToken: string;
  }