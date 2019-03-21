import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Logger, AuthenticationService, NotificationsService } from '@app/core';

@Component({
  selector: 'app-email-not-confirmed',
  templateUrl: './email-not-confirmed.component.html',
  styleUrls: ['./email-not-confirmed.component.scss']
})
export class EmailNotConfirmedComponent implements OnInit {

  isLoading = true;
  email = "";

  constructor(private router: Router,
              private route: ActivatedRoute,
              private authenticationService: AuthenticationService,
              private notifications: NotificationsService) {
  }

  ngOnInit(): void {
    this.email = this.route.snapshot.queryParams.email;
  }

  resendEmail() {
    this.authenticationService.sendEmailConfirmation(this.email).subscribe(() => {
      this.notifications.showMessage("New email sent");
      this.router.navigate(["/login"]);
    });
  }
}
