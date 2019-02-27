import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Credentials, AuthorizationService } from '../authorization/authorization.service';

export interface LoginContext {
  username: string;
  password: string;
  remember?: boolean;
}

export interface RegistrationContext {
  email: string;
  password: string;
  confirmPassword: string;
}

export interface ChangePasswordContext {
  oldPassword: string;
  newPassword: string;
  confirmPassword: string;
}

interface SignInResult {
  userName: string;
  caveat: string;
  token: string;
}

/**
 * Provides a base for authentication workflow.
 * The Credentials interface as well as login/logout methods should be replaced with proper implementation.
 */
@Injectable()
export class AuthenticationService {


  constructor(private _http: HttpClient, private _authorizationService: AuthorizationService) { }

  /**
   * Authenticates the user.
   * @param context The login parameters.
   * @return The user credentials.
   */
  login(context: LoginContext): Observable<Credentials> {
    return this._http
      .disableApiPrefix()
      .post<SignInResult>("account/login", { email: context.username, password: context.password })
      .pipe(map(result => this.processToken(result.token, context.username, context.remember)));
  }

  register(context: RegistrationContext): Observable<Credentials> {
    return this._http
      .disableApiPrefix()
      .post("account/register", context, { responseType: 'text' })
      .pipe(map(token => this.processToken(token, context.email, true)));
  }

  changePassword(context: ChangePasswordContext): Observable<any> {
    return this._http
      .disableApiPrefix()
      .put("account/password", context);
  }

  /**
   * Logs out the user and clear credentials.
   * @return True if the user was logged out successfully.
   */
  logout(): Observable<boolean> {
    // Customize credentials invalidation here
    this._authorizationService.setCredentials();
    return of(true);
  }

  private processToken(token: string, userName: string, remember: boolean) : Credentials {
    const credentials = {
      username: userName,
      token: token
    };
    this._authorizationService.setCredentials(credentials, remember);

    return credentials;
  }
}
