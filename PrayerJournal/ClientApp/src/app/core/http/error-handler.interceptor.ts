import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Observable, from, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { environment } from '@env/environment';
import { Logger } from '../logger.service';
import { NotificationsService } from '../notifications.service';
import { Router, ActivatedRoute } from '@angular/router';

const log = new Logger('ErrorHandlerInterceptor');

/**
 * Adds a default error handler to all requests.
 */
@Injectable()
export class ErrorHandlerInterceptor implements HttpInterceptor {

  constructor(private _notifications: NotificationsService, private router: Router) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(catchError(error => this.errorHandler(error)));
  }

  // Customize the default error handler here if needed
  private errorHandler(response: HttpErrorResponse): Observable<HttpEvent<any>> {
    if (!environment.production) {
      // Do something with the error
      log.error('Request error', response);
    }

    if (response.status === 0)
      this._notifications.showMessage("Network error: " + response.message);

    if (response.status === 401) {
      this._notifications.showMessage("Your login has expired or been revoked, please log in again.");
      this.router.navigate(['/login'], { queryParams: { redirect: this.router.url }, replaceUrl: true });
    }

    if (response.error.errors) {
      for (let key in response.error.errors)
        console.log(key + ": " + response.error.errors[key]);
    }
    else
      console.log(response);

    return throwError(response);
  }
}
