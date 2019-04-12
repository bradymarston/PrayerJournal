import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService, SignInResult, Logger } from '../../core';

const log = new Logger('External-Login');

@Component({
  selector: 'app-external-login-callback',
  templateUrl: './external-login-callback.component.html',
  styleUrls: ['./external-login-callback.component.scss']
})
export class ExternalLoginCallbackComponent implements OnInit {
  provider: string;
  hasError = false;

  constructor(private route: ActivatedRoute, private authenticationService: AuthenticationService, private router: Router) { }

  ngOnInit() {
    let code = this.route.snapshot.queryParamMap.get("code");
    this.provider = this.route.snapshot.queryParamMap.get("state");
    this.authenticationService.externalLogin(code, this.provider).subscribe(
      signInResult => this.handleSuccess(signInResult),
      () => this.hasError = true
    );
  }

  handleSuccess(signInResponse: SignInResult) {
    log.debug(`${signInResponse.userId} successfully logged in using ${this.provider}.`);
    this.router.navigate(['/'], { replaceUrl: true });
  }
}
