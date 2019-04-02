import { Component, OnInit } from '@angular/core';

import { UserAdminService, roleNames, roleIdentifiers, AuthorizationService } from '../core';
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
  searchSubstrings: string[] = [];
  sorts = [
    { id: "lastName", description: "Last Name" },
    { id: "firstName", description: "First Name" },
    { id: "email", description: "Email Address" },
  ];
  currentSort = this.sorts[0];

  constructor(private _userAdminService: UserAdminService, private _authorizationService: AuthorizationService, private _dialogService: DialogService) { }

  ngOnInit() {
    this._userAdminService.getUsers().subscribe(u => this.users = u);
  }

  addRole(user: User, role: string) {
    user.roles.push(role);
    this._userAdminService.addRole(user.id, role).subscribe(
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
    let filteredUsers = this.users;

    this.searchSubstrings.forEach(s => filteredUsers = filteredUsers.filter(
      u =>
        u.email.toUpperCase().includes(s.toUpperCase()) ||
        u.firstName.toUpperCase().includes(s.toUpperCase()) ||
        u.lastName.toUpperCase().includes(s.toUpperCase())
    ));

    filteredUsers.sort((a, b) => {
      if (a[this.currentSort.id] > b[this.currentSort.id])
        return 1;
      if (a[this.currentSort.id] < b[this.currentSort.id])
        return -1;
      if (a[this.currentSort.id] === b[this.currentSort.id])
        return 0;
    })

    return filteredUsers;
  }

  updateSearch(searchString: string) {
    this.searchSubstrings = searchString.split(" ");
    console.log(this.searchSubstrings);
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
        this._userAdminService.deleteUser(user.id).subscribe(
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
        this._userAdminService.removeRole(user.id, role).subscribe(
          () => null,
          () => user.roles.push(role)
        );
      }
    });
  }
}
