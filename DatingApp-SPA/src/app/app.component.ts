import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { SwUpdate } from '@angular/service-worker';
import { interval } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  title = 'DatingApp-SPA';
  jwtHelper = new JwtHelperService();
  hasUpdate = false;
  constructor(public authService: AuthService,
    private swUpdate: SwUpdate,) {

  }

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token) {
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }

    const loggedUser = JSON.parse(localStorage.getItem('user'));

    if (loggedUser) {
      this.authService.loggedUser = loggedUser;
      this.authService.changeMemberPhoto(loggedUser.photoUrl);
    }

    // check for platform update
    if (this.swUpdate.isEnabled) {
      interval(6000).subscribe(() => this.swUpdate.checkForUpdate().then(() => {
        // checking for updates
        console.log('checking for updates');
        
      }));
    }
    this.swUpdate.available.subscribe(() => {
      this.hasUpdate = true;
    });

  }

  reloadSite(): void {
    location.reload();
  }
}
