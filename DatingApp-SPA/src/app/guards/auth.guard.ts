import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router, private alertifyService: AlertifyService) {

  }
  canActivate(): boolean {
    if (this.authService.loggedIn()) {
      return true;
    }

    this.alertifyService.error('Must be logged in!!');
    this.router.navigate(['/home']);
    return false;
  }

}
