import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService, Logger, NotificationsService } from '../../core';
import { finalize } from 'rxjs/operators';
import { PasswordValidators } from '../../common/validators/password.validators';
import { PasswordMatchErrorMatcher } from '../../core/error-matchers/PasswordMatchErrorMatcher';

const log = new Logger('Reset Password');

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {

  error: string;
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
    this.authenticationService.resetPassword(this.form.value, this.code)
      .pipe(finalize(() => {
        this.form.markAsPristine();
        this.isLoading = false;
      }))
      .subscribe(() => {
        log.debug(`${this.form.controls.email.value} reset password.`);
        this.notifications.showMessage("Password successfully reset")
        this.router.navigate(['/']);
      }, () => {
        log.debug(`Invalid attempt to reset password by ${this.form.controls.email.value}`);
        this.router.navigate(['/']);
      });
  }

  private createForm() {
    this.form = this.formBuilder.group({
      email: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['']
    }, { validators: [PasswordValidators.passwordsMatch] });
  }
}
