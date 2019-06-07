import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { finalize } from 'rxjs/operators';

import { Logger, AuthenticationService, NotificationsService, SignInResult } from '@app/core';
import { PasswordValidators } from '../../common/validators/password.validators';
import { PasswordMatchErrorMatcher } from '../../core/error-matchers/PasswordMatchErrorMatcher';
import { HttpErrorResponse } from '@angular/common/http';
import { BadRequestErrorDetails } from '../../common/bad-request-error-details';

const log = new Logger('Register');

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  errors: string[] = [];
  form: FormGroup;
  isLoading = false;

  passwordMatchErrorMatcher = new PasswordMatchErrorMatcher();

  constructor(private router: Router,
              private route: ActivatedRoute,
              private formBuilder: FormBuilder,
              private authenticationService: AuthenticationService,
              private notifications: NotificationsService) {
    this.createForm();
  }

  ngOnInit() { }

  register() {
    this.isLoading = true;
    this.errors = [];
    this.authenticationService.register(this.form.value)
      .pipe(finalize(() => {
        this.form.markAsPristine();
        this.isLoading = false;
      }))
      .subscribe(
        signInResponse => this.handleSuccess(signInResponse),
        errorResponse => this.handleError(errorResponse));
  }

  handleSuccess(signInResponse: SignInResult) {
    log.debug(`${signInResponse.name} successfully logged in`);
    this.notifications.showMessage("Registered " + signInResponse.name);
    this.route.queryParams.subscribe(
      params => this.router.navigate([params.redirect || '/'], { replaceUrl: true })
    );
  }

  handleError(response: HttpErrorResponse) {
    log.debug(`Registration error: ${response.error}`);
    if (response.error instanceof BadRequestErrorDetails)
      this.errors = response.error.errors;
  }

  private createForm() {
    this.form = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(100)]],
      confirmPassword: ['']
    }, { validators: [PasswordValidators.passwordsMatch] });
  }

}
