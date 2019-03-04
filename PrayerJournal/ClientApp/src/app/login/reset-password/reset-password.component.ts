import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService, Logger, NotificationsService } from '../../core';
import { finalize } from 'rxjs/operators';

const log = new Logger('Reset Password');

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {

  error: string;
  resetPasswordForm: FormGroup;
  isLoading = false;
  code: string;

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
    this.authenticationService.resetPassword(this.resetPasswordForm.value, this.code)
      .pipe(finalize(() => {
        this.resetPasswordForm.markAsPristine();
        this.isLoading = false;
      }))
      .subscribe(() => {
        log.debug(`${this.resetPasswordForm.controls.email.value} reset password.`);
        this.notifications.showMessage("Password successfully reset")
        this.router.navigate(['/']);
      }, () => {
        log.debug(`Invalid attempt to reset password by ${this.resetPasswordForm.controls.email.value}`);
        this.router.navigate(['/']);
      });
  }

  private createForm() {
    this.resetPasswordForm = this.formBuilder.group({
      email: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    });
  }
}
