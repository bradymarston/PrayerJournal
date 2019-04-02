import { Component, OnInit } from '@angular/core';
import { finalize } from 'rxjs/operators';

import { QuoteService } from './quote.service';
import { ActivatedRoute } from '@angular/router';
import { UserAdminService } from '../core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  quote: string;
  isLoading: boolean;

  constructor(private quoteService: QuoteService, private route: ActivatedRoute, private userAdminService: UserAdminService) { }

  ngOnInit() {
    this.route.queryParamMap.subscribe((params) => {
      console.log(params);
      if (params.get("refreshRoles"))
        this.refreshRoles();
    });

    this.isLoading = true;
    this.quoteService.getRandomQuote({ category: 'dev' })
      .pipe(finalize(() => { this.isLoading = false; }))
      .subscribe((quote: string) => { this.quote = quote; });
  }

  refreshRoles() {
    this.userAdminService.refreshRoles().subscribe();
  }
}
