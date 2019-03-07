export class BadRequestErrorDetails {
  type = "";
  title = "";
  errors: string[] = [];

  constructor(type: string, title: string) {
    this.type = type;
    this.title = title;
  };
}
