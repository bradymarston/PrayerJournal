import { Title } from '@angular/platform-browser';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ObservableMedia } from '@angular/flex-layout';

import { AuthorizationService, I18nService, AuthenticationService } from '@app/core';

@Component({
  selector: 'app-shell',
  templateUrl: './shell.component.html',
  styleUrls: ['./shell.component.scss']
})
export class ShellComponent implements OnInit {

  constructor(private router: Router,
              private titleService: Title,
              private media: ObservableMedia,
              public authorizationService: AuthorizationService,
              private authenticationService: AuthenticationService,
              private i18nService: I18nService) { }

  ngOnInit() { }

  setLanguage(language: string) {
    this.i18nService.language = language;
  }

  logoutThisDevice() {
    this.authenticationService.logoutThisDevice()
      .subscribe(() => this.router.navigate(['/login'], { replaceUrl: true }));
  }

  logoutAllDevices() {
    this.authenticationService.logoutAllDevices()
      .subscribe(() => this.router.navigate(['/login'], { replaceUrl: true }));
  }

  get name(): string | null {
    const userInfo = this.authorizationService.userInfo;
    return userInfo ? userInfo.name : null;
  }

  get languages(): string[] {
    return this.i18nService.supportedLanguages;
  }

  get isMobile(): boolean {
    return this.media.isActive('xs') || this.media.isActive('sm');
  }

  get title(): string {
    return this.titleService.getTitle();
  }
}
