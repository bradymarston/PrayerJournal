import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { AuthorizationService } from './authorization.service';
import { Router, ActivatedRoute } from '@angular/router';
import { User } from './../common/user';

@Injectable()
export class UserAdminService {
  private baseAddress = "user-admin";

  constructor(private _http: HttpClient, private _authorizationService: AuthorizationService, private _router: Router, private _activatedRoute: ActivatedRoute) { }

  getUsers(): Observable<User[]> {
    return this._http
      .get<User[]>(`${this.baseAddress}/users`);
  }

  deleteUser(userId: string): Observable<any> {
    return this._http
      .delete(`${this.baseAddress}/user/${userId}`);
  }

  addRole(userId: string, role: string): Observable<any> {
    return this._http
      .post(`${this.baseAddress}/roles/${userId}?role=${role}`, {});
  }

  removeRole(userId: string, role: string): Observable<any> {
    return this._http
      .delete(`${this.baseAddress}/roles/${userId}?role=${role}`);
  }

  refreshRoles(): Observable<void> {
    return this._http
      .get<string[]>(`${this.baseAddress}/roles`)
      .pipe(map(result => this._authorizationService.setRoles(result)));
  }
}
