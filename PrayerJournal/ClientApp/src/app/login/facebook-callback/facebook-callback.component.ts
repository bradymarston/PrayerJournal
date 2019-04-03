import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-facebook-callback',
  templateUrl: './facebook-callback.component.html',
  styleUrls: ['./facebook-callback.component.scss']
})
export class FacebookCallbackComponent implements OnInit {

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.snapshot.paramMap.keys.forEach(key => console.log(this.route.snapshot.paramMap.getAll(key)));
    this.route.snapshot.queryParamMap.keys.forEach(key => console.log(this.route.snapshot.queryParamMap.getAll(key)));
  }

}
