import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home';
import { SeatSelectionComponent } from './components/seat-selection/seat-selection';
import { MyBookingsComponent } from './components/my-bookings/my-bookings';
import { LoginComponent } from './components/login/login';
import { RegisterComponent } from './components/register/register';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'seats/:showtimeId', component: SeatSelectionComponent },
  { path: 'my-bookings', component: MyBookingsComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: '**', redirectTo: '' }
];