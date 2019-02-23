import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';

import { AuthorizationService } from '../authorizationService/authorization.service';

/**
 * Caches HTTP requests.
 * Use ExtendedHttpClient fluent API to configure caching for each request.
 */
@Injectable()
export class AuthorizationInterceptor implements HttpInterceptor {

  constructor(private _authorizationService: AuthorizationService) { }

  /**
   * Configures interceptor options
   * @param options If update option is enabled, forces request to be made and updates cache entry.
   * @return The configured instance.
   */

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this._authorizationService.isAuthenticated) {
      return next.handle(request);
    }

    const authRequest = request.clone({ headers: request.headers.append("Authorization", "Bearer " + this._authorizationService.credentials.token) });
    return next.handle(authRequest);
  }
}
