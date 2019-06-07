import { NgModule, Optional, SkipSelf } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS, HttpClient, HttpClientModule } from '@angular/common/http';
import { RouteReuseStrategy, RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { RouteReusableStrategy } from './route-reusable-strategy';
import { AuthenticationService } from './authentication/authentication.service';
import { AuthenticationGuard } from './authentication/authentication.guard';
import { I18nService } from './i18n.service';
import { HttpService } from './http/http.service';
import { HttpCacheService } from './http/http-cache.service';
import { ApiPrefixInterceptor } from './http/api-prefix.interceptor';
import { ErrorHandlerInterceptor } from './http/error-handler.interceptor';
import { CacheInterceptor } from './http/cache.interceptor';
import { AuthorizationInterceptor } from './http/authorization.interceptor';
import { AuthorizationService } from './authorization.service';
import { NotificationsService } from './notifications.service';
import { PasswordMatchErrorMatcher } from './error-matchers/PasswordMatchErrorMatcher';
import { DialogService } from './dialog.service';
import { RoleGuard } from './authentication/role.guard';
import { UserAdminService } from './user.admin.service';
import { ConstantsService } from './constants/constants.service';
import { UserProfileService } from './user-profile.service';
import { AccessTokenInterceptor } from './http/access.token.interceptor';

@NgModule({
  imports: [
    CommonModule,
    HttpClientModule,
    TranslateModule,
    RouterModule
  ],
  providers: [
    AuthenticationService,
    AuthenticationGuard,
    RoleGuard,
    AuthorizationService,
    I18nService,
    HttpCacheService,
    NotificationsService,
    AccessTokenInterceptor,
    ApiPrefixInterceptor,
    AuthorizationInterceptor,
    ErrorHandlerInterceptor,
    PasswordMatchErrorMatcher,
    DialogService,
    CacheInterceptor,
    ConstantsService,
    UserAdminService,
    UserProfileService,
    {
      provide: HttpClient,
      useClass: HttpService
    },
    {
      provide: RouteReuseStrategy,
      useClass: RouteReusableStrategy
    }
  ]
})
export class CoreModule {

  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    // Import guard
    if (parentModule) {
      throw new Error(`${parentModule} has already been loaded. Import Core module in the AppModule only.`);
    }
  }

}
