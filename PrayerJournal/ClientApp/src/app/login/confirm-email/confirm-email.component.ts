import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Logger, AuthenticationService, NotificationsService } from '@app/core';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.scss']
})
export class ConfirmEmailComponent implements OnInit {

  isLoading = true;

  constructor(private router: Router,
              private route: ActivatedRoute,
              private authenticationService: AuthenticationService,
              private notifications: NotificationsService) {
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params =>
      this.authenticationService.confirmEmail(params.userId, params.code)
        .subscribe(
          () => {
            this.notifications.showMessage("Email confirmed");
            this.router.navigate(["/home"]);
          },
          () => this.isLoading = false)
    );
  }
}
