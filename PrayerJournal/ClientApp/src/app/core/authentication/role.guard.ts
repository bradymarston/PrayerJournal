import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

import { Logger } from '../logger.service';
import { AuthorizationService } from '../authorization.service';

const log = new Logger('AuthenticationGuard');

@Injectable()
export class RoleGuard implements CanActivate {

  constructor(private router: Router,
              private authorizationService: AuthorizationService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (this.authorizationService.isAuthenticated()) {
      let expectedRoles: string[] = route.data.expectedRoles;

      let passed = false;
      expectedRoles.forEach((role) => {
        if (this.authorizationService.isInRole(role))
          passed = true;
      });

      if (passed)
        return true;
    }

    log.debug('Does not have the role to access this resource. Redirecting.');
    this.router.navigate(['/'], { replaceUrl: true });
    return false;
  }

}
