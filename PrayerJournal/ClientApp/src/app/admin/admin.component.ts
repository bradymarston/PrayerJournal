import { Component, OnInit } from '@angular/core';

import { UserAdminService, roleNames, roleIdentifiers, AuthorizationService, ConstantsService } from '../core';
import { User } from '../common/user';
import { DialogService } from '../core/dialog.service';
import { SortedAndFilteredData } from '../common/sorted-and-filtered-data';
import { MatSelectChange } from '@angular/material';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {

  users: SortedAndFilteredData<User> = new SortedAndFilteredData<User>([], [], ["lastName", "firstName", "email"], ["lastName", "firstName", "email"]);
  roleIdentifiers = roleIdentifiers;
  roleNames = roleNames;
  searchSubstrings: string[] = [];
  sorts = [
    { id: "lastName", description: "Last Name" },
    { id: "firstName", description: "First Name" },
    { id: "email", description: "Email Address" },
  ];
  currentSort = this.sorts[0];

  constructor(private _userAdminService: UserAdminService, private _authorizationService: AuthorizationService, private _dialogService: DialogService, public constantsService: ConstantsService) { }

  ngOnInit() {
    this._userAdminService.getUsers().subscribe(u => {
      this.users.data = u;
      console.log(this.users.data);
      console.log(this.users.list);
    });
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
    return this.users.data.length === 0;
  }

  updateSearch(searchString: string) {
    this.searchSubstrings = searchString.split(" ");
    this.users.filterStrings = this.searchSubstrings;
  }

  updateSort(details: MatSelectChange) {
    let newSorts = this.sorts.slice(0);
    let index = newSorts.indexOf(details.value);
    newSorts.splice(index, 1);

    newSorts.reverse();
    newSorts.push(details.value);
    newSorts.reverse();

    this.users.sortByKeys = newSorts.map(sort => sort.id);
  }

  deleteUser(user: User) {
    this._dialogService.confirm({
      title: "DELETE USER CONFIRMATION",
      content: `Permenantly delete ${user.firstName} ${user.lastName}? (This cannot be undone)`,
      yesText: "DELETE",
      noText: "CANCEL",
      yesColor: "warn"
    }).subscribe(result => {
      if (result) {
        let index = this.users.remove(user);
        this._userAdminService.deleteUser(user.id).subscribe(
          () => null,
          () => this.users.add(user, index)
        );
      }
    });
  }

  removeRole(user: User, role: string) {
    this._dialogService.confirm({
      title: "REMOVE ROLE CONFIRMATION",
      content: `Remove the role "${this.roleNames[role]}" from ${user.firstName} ${user.lastName}?`,
      yesText: "REMOVE",
      noText: "CANCEL",
      yesColor: "warn"
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
