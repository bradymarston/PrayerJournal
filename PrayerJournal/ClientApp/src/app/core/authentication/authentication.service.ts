import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { AuthorizationService } from '../authorization.service';
import { Router, ActivatedRoute } from '@angular/router';
import { ResetPasswordComponent } from '../../login/reset-password/reset-password.component';

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

export interface ResetPasswordContext {
  email: string;
  password: string;
  confirmPassword: string;
}

export interface SignInResult {
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


  constructor(private _http: HttpClient, private _authorizationService: AuthorizationService, private _router: Router, private _activatedRoute: ActivatedRoute) { }

  /**
   * Authenticates the user.
   * @param context The login parameters.
   * @return The user credentials.
   */
  login(context: LoginContext): Observable<SignInResult> {
    return this._http
      .disableApiPrefix()
      .post<SignInResult>("account/login", { email: context.username, password: context.password })
      .pipe(map(result => this.processToken(result, context.remember)));
  }

  register(context: RegistrationContext): Observable<SignInResult> {
    return this._http
      .disableApiPrefix()
      .post<SignInResult>("account/register", context)
      .pipe(map(result => this.processToken(result, false)));
  }

  changePassword(context: ChangePasswordContext): Observable<any> {
    return this._http
      .disableApiPrefix()
      .put<SignInResult>("account/password", context)
      .pipe(map(result => this.processToken(result, this._authorizationService.isRemembered)));
  }

  confirmEmail(userId: string, code: string) {
    return this._http
      .disableApiPrefix()
      .put<SignInResult>("account/confirm-email?userId=" + userId + "&code=" + code, null)
      .pipe(map(result => this.processToken(result, false)));
  }

  sendEmailConfirmation(): Observable<any> {
    return this._http
      .disableApiPrefix()
      .get("account/confirm-email");
  }

  forgotPassword(email: string): Observable<any> {
    return this._http
      .disableApiPrefix()
      .get("account/password/" + email);
  }

  resetPassword(context: ResetPasswordContext, code: string): Observable<any> {
    return this._http
      .disableApiPrefix()
      .post("account/password?code=" + code, context);
  }

  /**
   * Logs out the user and clear credentials.
   * @return True if the user was logged out successfully.
   */
  logout(): Observable<boolean> {
    // Customize credentials invalidation here
    this._authorizationService.clearCredentials();
    return of(true);
  }

  private processToken(signInResult: SignInResult, remember: boolean) : SignInResult {
    const credentials = {
      username: signInResult.userName,
      caveat: signInResult.caveat,
      token: signInResult.token
    };

    this._authorizationService.setCredentials(credentials, remember);

    if (signInResult.caveat === "ChangePassword") {
      this._router.navigate(["/account/change-password"], { queryParamsHandling: "preserve" });
      return signInResult;
    }

    return signInResult;
  }
}
