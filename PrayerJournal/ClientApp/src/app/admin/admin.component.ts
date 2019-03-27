import { Component, OnInit } from '@angular/core';

import { AuthenticationService, roleNames, roleIdentifiers, AuthorizationService } from '../core';
import { User } from '../common/user';
import { DialogService } from '../core/dialog.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {

  users: User[] = [];
  roleIdentifiers = roleIdentifiers;
  roleNames = roleNames;

  constructor(private _authenticationService: AuthenticationService, private _authorizationService: AuthorizationService, private _dialogService: DialogService) { }

  ngOnInit() {
    this._authenticationService.getUsers().subscribe(u => this.users = u);
  }

  addRole(user: User, role: string) {
    user.roles.push(role);
    this._authenticationService.addRole(user.id, role).subscribe(
      () => null,
      () => {
        let index = user.roles.indexOf(role);
        user.roles.splice(index, 1);
      }
    );
  }

  get isLoading(): boolean {
    return this.users.length === 0;
  }

  get filteredUsers(): User[] {
    return this.users;
  }

  deleteUser(user: User) {
    this._dialogService.confirm({
      title: "Delete User Confirmation",
      content: `Permenantly delete ${user.firstName} ${user.lastName}? (This cannot be undone)`,
      yesText: "DELETE",
      noText: "CANCEL",
      yesColor: "warn",
      noColor: "basic"
    }).subscribe(result => {
      if (result) {
        let index = this.users.indexOf(user);
        this.users.splice(index, 1);
        this._authenticationService.deleteUser(user.id).subscribe(
          () => null,
          () => this.users.push(user)
        );
      }
    });
  }

  removeRole(user: User, role: string) {
    this._dialogService.confirm({
      title: "Remove Role Confirmation",
      content: `Remove the role "${this.roleNames[role]}" from ${user.firstName} ${user.lastName}?`,
      yesText: "REMOVE",
      noText: "CANCEL",
      yesColor: "warn",
      noColor: "basic"
    }).subscribe(result => {
      if (result) {
        let index = user.roles.indexOf(role);
        user.roles.splice(index, 1);
        this._authenticationService.removeRole(user.id, role).subscribe(
          () => null,
          () => user.roles.push(role)
        );
      }
    });
  }
}
