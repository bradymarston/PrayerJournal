import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Logger, AuthenticationService, NotificationsService, ConfirmEmailContext } from '@app/core';
import { BadRequestErrorDetails } from '../../common/bad-request-error-details';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.scss']
})
export class ConfirmEmailComponent implements OnInit {

  isLoading = true;
  errors: string[] = [];
  context: ConfirmEmailContext = {
    userId: "",
    code: "",
    password: ""
  };

  constructor(private router: Router,
              private route: ActivatedRoute,
              private authenticationService: AuthenticationService,
              private notifications: NotificationsService) {
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.context.userId = params.userId;
      this.context.code = params.code;

      this.isLoading = false;
    });
  }

  submit() {
    this.authenticationService.confirmEmail(this.context)
      .subscribe(
        () => {
          console.log("Confirm email: Success");
          this.notifications.showMessage("Email confirmed");
          this.router.navigate(["/home"]);
        },
        response => {
          if (response.error instanceof BadRequestErrorDetails)
            this.errors = response.error.errors;
        }
      );
  }
}
