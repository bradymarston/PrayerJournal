import { AbstractControl, FormGroup } from "@angular/forms";

export class PasswordValidators {
  static passwordsMatch(control: AbstractControl) {
    let password = (control as FormGroup).controls.password;

    if (!password)
      password = (control as FormGroup).controls.newPassword;

    let confirmPassword = (control as FormGroup).controls.confirmPassword;

    if (password.value === confirmPassword.value)
        return null;

    return { passwordsMatch: { valid: false } };
  }
}
