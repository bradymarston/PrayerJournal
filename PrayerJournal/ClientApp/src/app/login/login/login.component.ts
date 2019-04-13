import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { finalize } from 'rxjs/operators';

import { environment } from '@env/environment';
import { Logger, AuthenticationService, SignInResult, ConstantsService } from '@app/core';
import { HttpErrorResponse } from '@angular/common/http';
import { BadRequestErrorDetails } from '../../common/bad-request-error-details';
import { SignInErrorDetails } from '../../common/sign-in-error-details';

const log = new Logger('Login');

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  version: string = environment.version;
  errors: string[] = [];
  form: FormGroup;
  isLoading = false;
  redirectUrl: any;

  constructor(private router: Router,
              private route: ActivatedRoute,
              private formBuilder: FormBuilder,
              private authenticationService: AuthenticationService,
              public constantsService: ConstantsService
            ) {
    this.createForm();
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => this.redirectUrl = params.redirect);
  }

  login() {
    this.isLoading = true;
    this.errors = [];
    this.authenticationService.login(this.form.value)
      .pipe(finalize(() => {
        this.form.markAsPristine();
        this.isLoading = false;
      }))
      .subscribe(
        signInResponse => this.handleSuccess(signInResponse),
        errorResponse => this.handleError(errorResponse));
  }

  handleSuccess(signInResponse: SignInResult) {
    log.debug(`${signInResponse.userId} successfully logged in`);
    this.router.navigate([this.redirectUrl || '/'], { replaceUrl: true });
  }

  handleError(response: HttpErrorResponse) {
    log.debug(`Login error: ${response.error}`);
    if (response.error instanceof BadRequestErrorDetails)
      this.errors = response.error.errors;

    if (response.error instanceof SignInErrorDetails) {
      switch (response.error.reason) {
        case "CredentialUnconfirmed":
          this.router.navigate(["/email-not-confirmed"], { queryParams: { email: this.form.controls.email.value } });
          break;
        case "LockedOut":
          this.errors = ["This account has been locked out, please try again later."];
          break;
        default:
          console.log(response.error);
      }
    }
  }

  private createForm() {
    this.form = this.formBuilder.group({
      email: ['', Validators.required],
      password: ['', Validators.required],
      remember: true
    });
  }
}
