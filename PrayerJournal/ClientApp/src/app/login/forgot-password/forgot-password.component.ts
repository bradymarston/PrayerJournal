import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { finalize } from 'rxjs/operators';

import { Logger, AuthenticationService } from '@app/core';

const log = new Logger('Forgot Password');

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent implements OnInit {

  form: FormGroup;
  isLoading = false;

  constructor(private router: Router,
              private formBuilder: FormBuilder,
              private authenticationService: AuthenticationService) {
    this.createForm();
  }

  ngOnInit() { }

  submitEmail() {
    this.isLoading = true;
    this.authenticationService.forgotPassword(this.form.controls.email.value)
      .pipe(finalize(() => {
        this.form.markAsPristine();
        this.isLoading = false;
      }))
      .subscribe(
        () => this.handleSuccess(),
        () => this.handleError());
  }

  handleSuccess() {
    log.debug(`Password reset successfully requested for ${this.form.controls.email.value}.`);
    this.router.navigate(['/confirm-forgot-password'], { queryParamsHandling: "preserve" });
  }

  handleError() {
    log.debug(`Password reset request for ${this.form.controls.email.value} failed.`);
    this.router.navigate(['/confirm-forgot-password'], { queryParamsHandling: "preserve" });
  }

  private createForm() {
    this.form = this.formBuilder.group({
      email: ['', Validators.required]
    });
  }
}
