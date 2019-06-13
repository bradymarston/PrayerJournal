export class User {
  id = "";
  firstName = "";
  lastName = "";
  email = "";
  emailConfirmed = false;
  phoneNumber = "";
  phoneNumberConfirmed = false;
  pendingEmail = "";
  pendingPhoneNumber = "";
  hasPassword = false;
  roles: string[] = [];
  externalLogins: string[] = [];
}
