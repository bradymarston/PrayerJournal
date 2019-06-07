import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';

import { AuthorizationService } from '../authorization.service';

/**
 * Adds Authorization header to each request.
 */
@Injectable()
export class AuthorizationInterceptor implements HttpInterceptor {

  constructor(private _authorizationService: AuthorizationService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this._authorizationService.isAuthenticated()) {
      return next.handle(request);
    }

    const authRequest = request.clone({ headers: request.headers.append("Authorization", "Shady " + this._authorizationService.authInfo.token) });
    return next.handle(authRequest);
  }
}
