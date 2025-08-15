import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private toastr: ToastrService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const user = localStorage.getItem('user');
    let authReq = req;

    if (user) {
      const token = JSON.parse(user).token;
      authReq = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

    return next.handle(authReq).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          this.toastr.error('You are not logged in.');
        }
        return throwError(() => error);
      })
    );
  }
}