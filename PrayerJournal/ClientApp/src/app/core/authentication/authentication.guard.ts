import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

import { Logger } from '../logger.service';
import { AuthorizationService } from '../authorization.service';

const log = new Logger('AuthenticationGuard');

@Injectable()
export class AuthenticationGuard implements CanActivate {

  constructor(private router: Router,
              private authorizationService: AuthorizationService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    console.log(state.url);
    if (this.authorizationService.isAuthenticated()) {

      switch (this.authorizationService.credentials.caveat) {
        case "ConfirmEmail":
          if (state.url.startsWith('/email-not-confirmed'))
            return true;

          this.router.navigate(['/email-not-confirmed'], { queryParams: { redirect: state.url }, replaceUrl: true });
          return false;
        case "ChangePassword":
          if (state.url.startsWith('/account/change-password'))
            return true;

          this.router.navigate(['/account/change-password'], { queryParams: { redirect: state.url }, replaceUrl: true });
          return false;
        default:
          return true;
      }
    }

    log.debug('Not authenticated, redirecting and adding redirect url...');
    this.router.navigate(['/login'], { queryParams: { redirect: state.url }, replaceUrl: true });
    return false;
  }

}
