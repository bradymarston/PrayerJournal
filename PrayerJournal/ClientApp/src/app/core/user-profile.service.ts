import { Injectable } from "@angular/core";
import { User } from "../common/user";
import { HttpClient } from "@angular/common/http";

@Injectable()
export class UserProfileService {
  private baseAddress = "account";

  constructor(private _http: HttpClient) { }

  getProfile() {
    return this._http
      .disableApiPrefix()
      .get<User>(`${this.baseAddress}/profile`);
  }
}
