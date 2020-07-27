import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { User } from '../models/user';
import { UserService } from '../services/user.service';
import { AlertifyService } from '../services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Injectable } from '@angular/core';

@Injectable()
export class MemberListResolver implements Resolve<User[]>{
    constructor(private userService: UserService, private alertifyService: AlertifyService, private router: Router) { }

    resolve(): Observable<User[]> {
        return this.userService.getUsers().pipe(
            catchError(error => {
                this.alertifyService.error(error);
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }

}
