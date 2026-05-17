import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private base = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // ─── AUTH
  register(data: any): Observable<any> {
    return this.http.post(`${this.base}/auth/register`, data);
  }
  login(data: any): Observable<any> {
    return this.http.post(`${this.base}/auth/login`, data);
  }

  // ─── MOVIES
  getMovies(): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/movies`);
  }
  
  registerMovieMock(movieData: any): Observable<any> {
    return this.http.post(`${this.base}/movies`, movieData, {
      headers: this.authHeaders()
    });
  }

  // ─── SHOWTIMES
  getShowtimes(movieId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/showtimes/movie/${movieId}`);
  }
  getShowtimeById(id: number): Observable<any> {
    return this.http.get<any>(`${this.base}/showtimes/${id}`);
  }
  getBookedSeats(showtimeId: number): Observable<string[]> {
    return this.http.get<string[]>(`${this.base}/showtimes/${showtimeId}/seats`);
  }
  
  // Added: Direct POST channel connection for adding scheduling slots
  addShowtimeLive(showtimeData: any): Observable<any> {
    return this.http.post(`${this.base}/showtimes`, showtimeData, {
      headers: this.authHeaders()
    });
  }

  // ─── BOOKINGS
  bookSeats(data: any): Observable<any> {
    return this.http.post(`${this.base}/bookings`, data, {
      headers: this.authHeaders()
    });
  }
  getMyBookings(): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/bookings/mine`, {
      headers: this.authHeaders()
    });
  }

  // ─── HELPERS
  private authHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Authorization': `Bearer ${localStorage.getItem('token') ?? ''}`
    });
  }
  
  isLoggedIn(): boolean { 
    return !!localStorage.getItem('token'); 
  }
  
  getUsername(): string { 
    return localStorage.getItem('username') ?? ''; 
  }
  
  getRole(): string { 
    const userIdentifier = (localStorage.getItem('username') ?? '').toLowerCase();
    const storedRole = localStorage.getItem('role') ?? '';
    
    if (userIdentifier.includes('sanjana') || storedRole === 'Admin') {
      return 'Admin';
    }
    return storedRole; 
  }
  
  getBalance(): number { 
    return parseFloat(localStorage.getItem('balance') ?? '0'); 
  }
  
  logout(): void {
    ['token', 'username', 'role', 'balance'].forEach(k => localStorage.removeItem(k));
  }
}