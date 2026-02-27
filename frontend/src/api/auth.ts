import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5000/api',
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

export interface AuthResponse {
  token: string;
  user: { id: string; firstName: string; lastName: string; email: string; createdAt: string };
}

export const registerUser = (data: { firstName: string; lastName: string; email: string; password: string }) =>
  api.post<AuthResponse>('/auth/register', data);

export const loginUser = (data: { email: string; password: string }) =>
  api.post<AuthResponse>('/auth/login', data);

export const getMe = () => api.get<AuthResponse['user']>('/auth/me');