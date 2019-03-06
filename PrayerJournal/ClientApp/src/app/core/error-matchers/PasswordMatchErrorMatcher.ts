import { ErrorStateMatcher } from "@angular/material";
import { FormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { Injectable } from "@angular/core";

@Injectable()
export class PasswordMatchErrorMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl, form: FormGroupDirective | NgForm): boolean {
    return control.touched && form.hasError("passwordsMatch");
  }
}
