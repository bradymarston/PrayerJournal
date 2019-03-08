import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Observable, from, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { environment } from '@env/environment';
import { Logger } from '../logger.service';
import { NotificationsService } from '../notifications.service';
import { Router, ActivatedRoute } from '@angular/router';
import { BadRequestErrorDetails } from '../../common/bad-request-error-details';
import { AuthorizationService } from '../authorization.service';

const log = new Logger('ErrorHandlerInterceptor');

/**
 * Adds a default error handler to all requests.
 */
@Injectable()
export class ErrorHandlerInterceptor implements HttpInterceptor {

  constructor(private _notifications: NotificationsService, private router: Router, private _authorizationService: AuthorizationService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(catchError(error => this.errorHandler(error)));
  }

  // Customize the default error handler here if needed
  private errorHandler(response: HttpErrorResponse): Observable<HttpEvent<any>> {
    let processedResponse: any;

    switch (response.status) {
      case 0:
        processedResponse = this.handleNetworkError(response);
        break;
      case 401:
        processedResponse = this.handleUnathorized(response);
        break;
      case 500:
        processedResponse = this.handleServerError(response);
        break;
      case 400:
        processedResponse = this.handleBadRequest(response);
        break;
      default:
        processedResponse = this.handleUnknownErrorType(response);
    }

    return throwError(processedResponse);
  }

  handleNetworkError(response: HttpErrorResponse): HttpErrorResponse {
    this._notifications.showMessage("Network error: " + response.message);
    return response;
  }

  handleUnathorized(response: HttpErrorResponse): HttpErrorResponse {
    this._notifications.showMessage("Your login has expired or been revoked, please log in again.");
    this._authorizationService.clearCredentials();
    this.router.navigate(['/login'], { queryParams: { redirect: this.router.url }, replaceUrl: true });
    return response;
  }

  handleServerError(response: HttpErrorResponse): HttpErrorResponse {
    this._notifications.showMessage("Server Error: There was an error on the server processing your request. If you think this an error, please report it.");
    return response;
  }

  handleBadRequest(response: HttpErrorResponse): HttpErrorResponse {
    if (response.error.errors && response.error.type) {
      let error = new BadRequestErrorDetails(response.error.type, response.error.title);

      for (let key in response.error.errors) 
        for (let errorKey in response.error.errors[key])
          error.errors.push(response.error.errors[key][errorKey]);

      return new HttpErrorResponse({
        error: error,
        headers: response.headers,
        status: response.status,
        statusText: response.statusText,
        url: response.url
      })
    }
    return response;
  }

  handleUnknownErrorType(response: HttpErrorResponse): HttpErrorResponse {
    this._notifications.showMessage("Server Request Error (" + response.status + "): " + response.message)
    return response;
  }
}
