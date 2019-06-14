import { Component, OnInit } from '@angular/core';
import { User } from '../../common/user';
import { UserProfileService, NotificationsService } from '../../core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BadRequestErrorDetails } from '../../common/bad-request-error-details';
import { DialogService } from '../../core/dialog.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  user = new User();
  errors: string[] = [];
  form: FormGroup;
  isLoading = false;
  isUpdating = false;

  constructor(private userProfileService: UserProfileService, private formBuilder: FormBuilder, private dialogService: DialogService, private notificaitonService: NotificationsService) {
    this.createForm();
  }

  ngOnInit() {
    this.userProfileService.getProfile().subscribe(u => {
      this.user = u;
      this.form.controls.firstName.setValue(u.firstName);
      this.form.controls.lastName.setValue(u.lastName);
      this.form.controls.email.setValue(u.email);
      this.form.markAsPristine();
    });
  }

  private createForm() {
    this.form = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['']
    });
  }

  updateProfile() {
    this.isUpdating = true;
    var userUpdates = new User();

    userUpdates.firstName = this.form.controls.firstName.value;
    userUpdates.lastName = this.form.controls.lastName.value;

    this.userProfileService.updateProfile(userUpdates).subscribe(
      () => {
        this.user.firstName = userUpdates.firstName;
        this.user.lastName = userUpdates.lastName;
        this.form.markAsPristine();
        this.isUpdating = false;
      },
      () => {
        this.form.controls.firstName.setValue(this.user.firstName);
        this.form.controls.lastName.setValue(this.user.lastName);
        this.isUpdating = false;
      });
  }

  resendEmailConfirmation() {
    this.userProfileService.resendEmailConfirmation().subscribe(() => this.notificaitonService.showMessage("Confirmation sent."));
  }

  cancelEmailChange() {
    const oldPendingEmail = this.user.pendingEmail;

    this.user.pendingEmail = null;

    this.userProfileService.cancelEmailChange().subscribe(
      () => { },
      () => this.user.pendingEmail = oldPendingEmail
    );
  }

  changeEmail() {
    this.dialogService.prompt({
      title: "CHANGE EMAIL ADDRESS",
      content: "Enter new email address",
      placeholder: "Email Address"
    }).subscribe(newEmail => {
      const oldPendingEmail = this.user.pendingEmail;

      if (newEmail) {
        this.user.pendingEmail = newEmail;
        this.userProfileService.setEmail(newEmail).subscribe(
          () => {
            this.dialogService.message({
              title: "CHANGE SUBMITTED",
              content: "You have been sent a confirmation email. Check your email and follow the directions."
            }).subscribe();
          },
          response => {
            this.user.pendingEmail = oldPendingEmail;
            if (response.error instanceof BadRequestErrorDetails)
              this.errors = response.error.errors;
          }
        );
      }
    });
  }
}
