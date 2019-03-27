import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material';

export class ConfirmDialogOptions {
  title = "Confirmation";
  content = "Which one?";
  yesText = "YES";
  noText = "NO";
  yesColor = "primary";
  noColor = "basic";
}

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.scss']
})
export class ConfirmDialogComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public data: ConfirmDialogOptions) {
    if (!this.data) this.data = new ConfirmDialogOptions();
  }

  ngOnInit() {
  }
}
