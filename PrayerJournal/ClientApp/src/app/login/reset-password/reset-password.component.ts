import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService, Logger, NotificationsService } from '../../core';
import { finalize } from 'rxjs/operators';
import { PasswordValidators } from '../../common/validators/password.validators';
import { PasswordMatchErrorMatcher } from '../../core/error-matchers/PasswordMatchErrorMatcher';
import { HttpErrorResponse } from '@angular/common/http';
import { BadRequestErrorDetails } from '../../common/bad-request-error-details';

const log = new Logger('Reset Password');

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {

  errors: string[] = [];
  form: FormGroup;
  isLoading = false;
  code: string;

  passwordMatchErrorMatcher = new PasswordMatchErrorMatcher();

  constructor(private router: Router,
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private authenticationService: AuthenticationService,
    private notifications: NotificationsService) {
    this.createForm();
  }

  ngOnInit() {
    this.route.queryParams.subscribe(qp => {
      this.code = qp["code"];
      if (!this.code)
        this.router.navigate(["/login"])
    });
  }

  resetPassword() {
    this.isLoading = true;
    this.errors = [];
    this.authenticationService.resetPassword(this.form.value, this.code)
      .pipe(finalize(() => {
        this.form.markAsPristine();
        this.isLoading = false;
      }))
      .subscribe(
        () => this.handleSuccess(),
        errorResponse => this.handleError(errorResponse));
  }

  handleSuccess() {
    log.debug(`${this.form.controls.email.value} potentially reset password.`);
    this.notifications.showMessage("Password reset successfully submitted")
    this.router.navigate(['/']);
  }

  handleError(response: HttpErrorResponse) {
    log.debug(`Invalid attempt to reset password by ${this.form.controls.email.value}: ${response.error}`);
    if (response.error instanceof BadRequestErrorDetails) {
      this.errors = response.error.errors;
    }
  }

  private createForm() {
    this.form = this.formBuilder.group({
      email: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(100)]],
      confirmPassword: ['']
    }, { validators: [PasswordValidators.passwordsMatch] });
  }
}
