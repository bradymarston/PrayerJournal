import { Component, OnInit } from '@angular/core';

import { ActivatedRoute } from '@angular/router';
import { UserAdminService } from '../core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  constructor(private route: ActivatedRoute, private userAdminService: UserAdminService) { }

  ngOnInit() {
    this.route.queryParamMap.subscribe((params) => {
      if (params.get("refreshRoles"))
        this.refreshRoles();
    });
  }

  refreshRoles() {
    this.userAdminService.refreshRoles().subscribe();
  }
}
