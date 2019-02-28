import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Logger, AuthenticationService, NotificationsService } from '@app/core';

@Component({
  selector: 'app-email-not-confirmed',
  templateUrl: './email-not-confirmed.component.html',
  styleUrls: ['./email-not-confirmed.component.scss']
})
export class EmailNotConfirmedComponent {

  isLoading = true;

  constructor(private router: Router,
              private route: ActivatedRoute,
              private authenticationService: AuthenticationService,
              private notifications: NotificationsService) {
  }

  resendEmail() {
    this.authenticationService.sendEmailConfirmation().subscribe(() => this.notifications.showMessage("New email sent"));
  }

  logout() {
    this.authenticationService.logout().subscribe(() => this.router.navigate(["/login"], { queryParamsHandling: "preserve" }));
  }
}
