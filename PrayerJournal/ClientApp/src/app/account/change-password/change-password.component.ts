import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { finalize } from 'rxjs/operators';

import { Logger, I18nService, AuthenticationService, AuthorizationService, NotificationsService } from '@app/core';

const log = new Logger('Change Password');

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit {

  error: string;
  changePasswordForm: FormGroup;
  isLoading = false;
  sentForCaveat = false;

  constructor(private router: Router,
              private route: ActivatedRoute,
              private formBuilder: FormBuilder,
              private i18nService: I18nService,
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
    this.authenticationService.changePassword(this.changePasswordForm.value)
      .pipe(finalize(() => {
        this.changePasswordForm.markAsPristine();
        this.isLoading = false;
      }))
      .subscribe(() => {
        log.debug(`${this.authorizationService.credentials.username} successfully changed their password`);
        this.notifications.showMessage("Password successfully changed");
        this.route.queryParams.subscribe(
          params => this.router.navigate([params.redirect || '/'], { replaceUrl: true })
        );
      }, error => {
        log.debug(`Registration error: ${error}`);
        this.error = error;
      });
  }

  get currentLanguage(): string {
    return this.i18nService.language;
  }

  get languages(): string[] {
    return this.i18nService.supportedLanguages;
  }

  private createForm() {
    this.changePasswordForm = this.formBuilder.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    });
  }
}
