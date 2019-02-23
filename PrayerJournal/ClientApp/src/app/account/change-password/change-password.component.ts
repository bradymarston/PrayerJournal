import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { finalize } from 'rxjs/operators';

import { environment } from '@env/environment';
import { Logger, I18nService, AuthenticationService } from '@app/core';

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

  constructor(private router: Router,
              private route: ActivatedRoute,
              private formBuilder: FormBuilder,
              private i18nService: I18nService,
              private authenticationService: AuthenticationService) {
    this.createForm();
  }

  ngOnInit() { }

  register() {
    this.isLoading = true;
    this.authenticationService.changePassword(this.changePasswordForm.value)
      .pipe(finalize(() => {
        this.changePasswordForm.markAsPristine();
        this.isLoading = false;
      }))
      .subscribe(() => {
        log.debug(`${this.authenticationService.credentials.username} successfully changed their password`);
        this.route.queryParams.subscribe(
          params => {
            if (params.redirect)
              this.router.navigate([params.redirect || '/'], { replaceUrl: true });
          }
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
