import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { finalize } from 'rxjs/operators';

import { Logger, AuthenticationService, AuthorizationService, NotificationsService } from '@app/core';
import { PasswordValidators } from '../../common/validators/password.validators';
import { PasswordMatchErrorMatcher } from '../../core/error-matchers/PasswordMatchErrorMatcher';
import { HttpErrorResponse } from '@angular/common/http';
import { BadRequestErrorDetails } from '../../common/bad-request-error-details';

const log = new Logger('Change Password');

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit {

  errors: string[] = [];
  form: FormGroup;
  isLoading = false;
  sentForCaveat = false;

  passwordMatchErrorMatcher = new PasswordMatchErrorMatcher();

  constructor(private router: Router,
              private route: ActivatedRoute,
              private formBuilder: FormBuilder,
              private authenticationService: AuthenticationService,
              private authorizationService: AuthorizationService,
              private notifications: NotificationsService) {
    this.createForm();
  }

  ngOnInit() {
    if (this.authorizationService.credentials.caveat === "ChangePassword") {
      this.sentForCaveat = true;
      this.authorizationService.clearCaveat();
    }
  }

  changePassword() {
    this.isLoading = true;
    this.errors = [];
    this.authenticationService.changePassword(this.form.value)
      .pipe(finalize(() => {
        this.form.markAsPristine();
        this.isLoading = false;
      }))
      .subscribe(
        () => this.handleSuccess(),
        errorResponse => this.handleError(errorResponse));
  }

  handleSuccess() {
    log.debug(`${this.authorizationService.credentials.username} successfully changed their password`);
    this.notifications.showMessage("Password successfully changed");
    this.route.queryParams.subscribe(
      params => this.router.navigate([params.redirect || '/'])
    );
  }

  handleError(response: HttpErrorResponse) {
    log.debug(`Password change error for ${this.authorizationService.credentials.username}: ${response.error}`);
    if (response.error instanceof BadRequestErrorDetails)
      this.errors = response.error.errors;
  }

  private createForm() {
    this.form = this.formBuilder.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', Validators.required],
      confirmPassword: ['']
    }, { validators: [PasswordValidators.passwordsMatch] });
  }
}
