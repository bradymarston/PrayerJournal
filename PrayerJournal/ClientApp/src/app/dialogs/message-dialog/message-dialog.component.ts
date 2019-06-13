import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';

export class MessageDialogOptions {
  title = "MESSAGE";
  content = "Read this";
  buttonText = "OKAY";
  buttonColor = "primary";
}

@Component({
  selector: 'app-message-dialog',
  templateUrl: './message-dialog.component.html',
  styleUrls: ['./message-dialog.component.scss']
})
export class MessageDialogComponent implements OnInit {
  value = "";

  constructor(@Inject(MAT_DIALOG_DATA) public data: MessageDialogOptions) {
    if (!this.data) this.data = new MessageDialogOptions();
  }

  ngOnInit() {
  }
}
