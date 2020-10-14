import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService,
    private router: Router, private alertifyService: AlertifyService) {

  }
  canActivate(next: ActivatedRouteSnapshot): boolean {
    const roles = next.firstChild.data['roles'] as Array<string>;
    if (roles) {
      const match = this.authService.roleMatch(roles);
      if (match) {
        return true;
      } else {
        this.router.navigate(['members']);
        this.alertifyService.error('You are not authorized to access this area');
      }
    }

    if (this.authService.loggedIn()) {
      return true;
    }

    this.alertifyService.error('Must be logged in!!');
    this.router.navigate(['/home']);
    return false;
  }

}
