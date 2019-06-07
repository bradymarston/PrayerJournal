import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, mergeMap } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { AuthorizationService, StoredUserInfo } from '../authorization.service';
import { Router, ActivatedRoute } from '@angular/router';

export interface LoginContext {
  email: string;
  password: string;
  remember?: boolean;
}

export interface RegistrationContext {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface ChangePasswordContext {
  oldPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export interface ResetPasswordContext {
  email: string;
  password: string;
  confirmPassword: string;
}

export interface SignInResult {
  name: string;
  hasPassword: boolean;
  caveat: string;
  roles: string[];
}

interface ExternalLoginUrl {
  url: string;
}

/**
 * Provides a base for authentication workflow.
 */
@Injectable()
export class AuthenticationService {
  private baseAddress = "account";

  constructor(private _http: HttpClient, private _authorizationService: AuthorizationService, private _router: Router, private _activatedRoute: ActivatedRoute) { }

  /**
   * Authenticates the user.
   * @param context The login parameters.
   * @return The user info.
   */
  login(context: LoginContext): Observable<SignInResult> {
    return this._http
      .disableApiPrefix()
      .post<SignInResult>(`${this.baseAddress}/login`, context)
      .pipe(map(result => this.processSignInResult(result, context.remember)));
  }

  register(context: RegistrationContext): Observable<SignInResult> {
    return this._http
      .disableApiPrefix()
      .post<SignInResult>(`${this.baseAddress}/register`, context)
      .pipe(map(result => this.processSignInResult(result, false)));
  }

  changePassword(context: ChangePasswordContext): Observable<any> {
    return this._http
      .disableApiPrefix()
      .put(`${this.baseAddress}/password`, context);
  }

  confirmEmail(userId: string, code: string) {
    return this._http
      .disableApiPrefix()
      .put<SignInResult>(`${this.baseAddress}/confirm-email?userId=${userId}&code=${code}`, null)
      .pipe(map(result => this.processSignInResult(result, false)));
  }

  sendEmailConfirmation(email: string): Observable<any> {
    return this._http
      .disableApiPrefix()
      .get(`${this.baseAddress}/confirm-email/${email}`);
  }

  forgotPassword(email: string): Observable<any> {
    return this._http
      .disableApiPrefix()
      .get(`${this.baseAddress}/password/${email}`);
  }

  resetPassword(context: ResetPasswordContext, code: string): Observable<any> {
    return this._http
      .disableApiPrefix()
      .post(`${this.baseAddress}/password?code=${code}`, context);
  }

  getExternalLoginUri(provider: string): Observable<string> {
    return this._http
      .disableApiPrefix()
      .get<ExternalLoginUrl>(`${this.baseAddress}/external-login/${provider}`)
      .pipe(map(result => result.url));
  }

  externalLogin(code: string, provider: string, securityState: string) {
    return this._http
      .disableApiPrefix()
      .post<SignInResult>(`${this.baseAddress}/external-login?code=${code}&state=${securityState}&provider=${provider}`, null)
      .pipe(map(result => this.processSignInResult(result, false)));
  }

  /**
   * Logs out the user and clear credentials.
   * @return True if the user was logged out successfully.
   */
  logoutThisDevice(): Observable<boolean> {
    // Customize credentials invalidation here
    this._authorizationService.clearCredentials();
    return of(true);
  }

  logoutAllDevices(): Observable<boolean> {
    return this._http
      .disableApiPrefix()
      .post(`${this.baseAddress}/logout`, null)
      .pipe(mergeMap(() => this.logoutThisDevice()));
  }

  private processSignInResult(signInResult: SignInResult, remember: boolean) : SignInResult {
    const userInfo: StoredUserInfo = {
      name: signInResult.name,
      hasPassword: signInResult.hasPassword,
      caveat: signInResult.caveat,
      roles: signInResult.roles
    };

    this._authorizationService.setUserInfo(userInfo, remember);

    if (signInResult.caveat === "ChangePassword") {
      this._router.navigate(["/account/change-password"], { queryParamsHandling: "preserve" });
      return signInResult;
    }

    return signInResult;
  }
}
