import { Component, OnInit, Input } from '@angular/core';
import { ExternalLoginProvider, AuthenticationService } from '../../core';

@Component({
  selector: 'app-external-login-button',
  templateUrl: './external-login-button.component.html',
  styleUrls: ['./external-login-button.component.scss']
})
export class ExternalLoginButtonComponent implements OnInit {
  @Input() provider: ExternalLoginProvider;
  @Input() redirectUrl: string;

  callbackUrl = "https://localhost:44306/external-login-callback";
  loginServiceUrl: string;

  constructor(private authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.loginServiceUrl = this.provider.codeUrl;
    this.loginServiceUrl += `?client_id=${this.provider.clientId}`;
    this.loginServiceUrl += `&redirect_uri=${this.callbackUrl}`;
    this.loginServiceUrl += `&state={%22provider%22:%22${this.provider.id}%22,%22redirect%22:%22${this.redirectUrl}%22}`;
    this.loginServiceUrl += `${this.provider.otherQueryParams}`
  }

  login() {
    this.authenticationService.getExternalLoginUri(this.provider.id).subscribe(uri => window.location.href = this.addInfoToState(uri));
  }

  addInfoToState(urlString: string): string {
    const url = new URL(urlString);
    const oldState = url.searchParams.get('state');
    const newState = `{"provider":"${this.provider.id}","redirect":"${this.redirectUrl}","securityState":"${oldState}"}`;
    url.searchParams.set('state', newState);
    return url.href;
  }
}
