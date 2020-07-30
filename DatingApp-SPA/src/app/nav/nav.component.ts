import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
  photoUrl: string;
  constructor(public authService: AuthService, private alertifyService: AlertifyService, private router: Router) { }

  ngOnInit() {
    this.authService.photoUrl.subscribe((photoUrl) => this.photoUrl = photoUrl);
  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      this.alertifyService.success("Logged in successfully");
    }, error => {
      this.alertifyService.error(error);

    }, () => {
      this.router.navigate(['/members']);
    });
  }

  loggedIn(): boolean {
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.loggedUser = null;
    this.alertifyService.message('logged out');
    this.router.navigate(['/home']);
  }

}
