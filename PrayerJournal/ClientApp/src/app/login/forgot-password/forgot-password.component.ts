import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { finalize } from 'rxjs/operators';

import { environment } from '@env/environment';
import { Logger, AuthenticationService } from '@app/core';

const log = new Logger('Forgot Password');

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent implements OnInit {

  forgotPasswordForm: FormGroup;
  isLoading = false;

  constructor(private router: Router,
              private route: ActivatedRoute,
              private formBuilder: FormBuilder,
              private authenticationService: AuthenticationService) {
    this.createForm();
  }

  ngOnInit() { }

  submitEmail() {
    this.isLoading = true;
    this.authenticationService.forgotPassword(this.forgotPasswordForm.controls.email.value)
      .pipe(finalize(() => {
        this.forgotPasswordForm.markAsPristine();
        this.isLoading = false;
      }))
      .subscribe(() => {
        log.debug(`Password reset successfully requested for ${this.forgotPasswordForm.controls.email.value}.`);
        this.router.navigate(['/confirm-forgot-password'], { queryParamsHandling: "preserve" });
      }, () => {
        log.debug(`Password reset request for ${this.forgotPasswordForm.controls.email.value} failed.`);
        this.router.navigate(['/confirm-forgot-password'], { queryParamsHandling: "preserve" });
      });
  }

  private createForm() {
    this.forgotPasswordForm = this.formBuilder.group({
      email: ['', Validators.required]
    });
  }
}