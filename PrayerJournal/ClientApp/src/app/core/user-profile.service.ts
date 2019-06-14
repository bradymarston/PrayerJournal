import { Injectable } from "@angular/core";
import { User } from "../common/user";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { AuthenticationService } from "./authentication/authentication.service";
import { SignInResult } from "./authentication/authentication.service";
import { map } from "rxjs/operators";

@Injectable()
export class UserProfileService {
  private baseAddress = "account";

  constructor(private _http: HttpClient, private authenticationService: AuthenticationService) { }

  getProfile() {
    return this._http
      .disableApiPrefix()
      .get<User>(`${this.baseAddress}/profile`);
  }

  updateProfile(user: User) {
    return this._http
      .disableApiPrefix()
      .put<SignInResult>(`${this.baseAddress}/profile`, user)
      .pipe(map(result => this.authenticationService.processSignInResult(result, false)));
  }

  resendEmailConfirmation() {
    return this._http
      .disableApiPrefix()
      .get(`${this.baseAddress}/confirm-email`);
  }

  cancelEmailChange() {
    return this._http
      .disableApiPrefix()
      .delete(`${this.baseAddress}/confirm-email`);
  }

  setEmail(email: string): Observable<any> {
    return this._http
      .disableApiPrefix()
      .post(`${this.baseAddress}/email/${email}`, null);
  }
}
