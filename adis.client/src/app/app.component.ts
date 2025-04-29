import { Component} from '@angular/core';
import { Router } from '@angular/router';
import { AuthStateService } from './services/auth-state.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.scss'
})

export class AppComponent {
  title = 'adis.client';

  constructor(public router: Router, public authService: AuthStateService) {}
}
