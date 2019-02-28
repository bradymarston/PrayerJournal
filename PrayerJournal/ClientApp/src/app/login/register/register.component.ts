import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { finalize } from 'rxjs/operators';

import { environment } from '@env/environment';
import { Logger, I18nService, AuthenticationService, NotificationsService } from '@app/core';

const log = new Logger('Login');

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  version: string = environment.version;
  error: string;
  registerForm: FormGroup;
  isLoading = false;

  constructor(private router: Router,
              private route: ActivatedRoute,
              private formBuilder: FormBuilder,
              private i18nService: I18nService,
              private authenticationService: AuthenticationService,
              private notifications: NotificationsService) {
    this.createForm();
  }

  ngOnInit() { }

  register() {
    this.isLoading = true;
    this.authenticationService.register(this.registerForm.value)
      .pipe(finalize(() => {
        this.registerForm.markAsPristine();
        this.isLoading = false;
      }))
      .subscribe(credentials => {
        log.debug(`${credentials.userName} successfully logged in`);
        this.notifications.showMessage("Registered " + credentials.userName);
        this.route.queryParams.subscribe(
          params => this.router.navigate([ params.redirect || '/'], { replaceUrl: true })
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
    this.registerForm = this.formBuilder.group({
      email: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    });
  }

}
