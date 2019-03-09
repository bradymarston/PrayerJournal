export class SignInErrorDetails {
  type = "";
  title = "";
  reason = "";

  constructor(type: string, title: string, reason: string) {
    this.type = type;
    this.title = title;
    this.reason = reason;
  };
}
