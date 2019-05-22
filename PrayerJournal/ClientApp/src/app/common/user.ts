export class User {
  id = "";
  firstName = "";
  lastName = "";
  email = "";
  phoneNumber = "";
  hasPassword = false;
  roles: string[] = [];
  externalLogins: string[] = [];
}
