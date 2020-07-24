import { HttpInterceptor, HttpEvent, HttpHandler, HttpRequest, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export class ErrorInterCeptor implements HttpInterceptor {
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(
            catchError(errorResponse => {
                if (errorResponse.status === 401) {
                    return throwError(errorResponse.statusText);
                }
                if (errorResponse instanceof HttpErrorResponse) {
                    const applicationError = errorResponse.headers.get('Application-Error');
                    if (applicationError) {
                        return throwError(applicationError);
                    }
                    const serverError = errorResponse.error;
                    let modalStateErrors = '';
                    if (serverError.errors && typeof serverError.errors === 'object') {
                        for (const key in serverError.errors) {
                            if (Object.prototype.hasOwnProperty.call(serverError.errors, key)) {
                                modalStateErrors += serverError.errors[key] + '\n';
                            }
                        }
                    }
                    return throwError(modalStateErrors || serverError || 'Server Error');
                }
            })
        );
    }
}

export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterCeptor,
    multi: true
};
