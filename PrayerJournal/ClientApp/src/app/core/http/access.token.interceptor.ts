import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpResponse, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

import { AuthorizationService } from '../authorization.service';
import { tap } from 'rxjs/operators';

/**
 * Receives access token from responses.
 */
@Injectable()
export class AccessTokenInterceptor implements HttpInterceptor {

  constructor(private _authorizationService: AuthorizationService) { }

  private readonly accessTokenHeaderKey = "access-token";
  private readonly userIdHeaderKey = "user-id";
  private readonly isPersistentHeaderKey = "persist-login";

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(tap((event: HttpEvent<any>) => {
      if (event instanceof HttpResponse)
        this.storeAuthInfo(event.headers);
  }));
  }

  private storeAuthInfo(headers: HttpHeaders) {
    if (headers.has(this.accessTokenHeaderKey) && headers.has(this.userIdHeaderKey) && headers.has(this.isPersistentHeaderKey)) {
      let accessToken = headers.get(this.accessTokenHeaderKey);
      let userId = headers.get(this.userIdHeaderKey);
      let isPersistent = headers.get(this.isPersistentHeaderKey).toLowerCase() == "true";

      this._authorizationService.setAuthInfo({ userId: userId, token: accessToken }, isPersistent);
    }
  }
}
