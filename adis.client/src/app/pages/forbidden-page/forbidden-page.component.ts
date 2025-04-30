import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-forbidden-page',
  imports: [
    MatIconModule,
    MatCardModule,
    MatButtonModule,
    RouterModule
  ],
  templateUrl: './forbidden-page.component.html',
  styleUrl: './forbidden-page.component.scss'
})
export class ForbiddenPageComponent {

}
