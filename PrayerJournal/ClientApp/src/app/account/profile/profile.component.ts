import { Component, OnInit } from '@angular/core';
import { User } from '../../common/user';
import { UserProfileService } from '../../core';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  user: User;

  constructor(private userProfileService: UserProfileService) { }

  ngOnInit() {
    this.userProfileService.getProfile().subscribe(u => this.user = u);
  }
}
