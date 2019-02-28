import { Injectable } from "@angular/core";
import { MatSnackBar } from "@angular/material";

@Injectable()
export class NotificationsService {
  constructor(private _snackBar: MatSnackBar) { }

  public showMessage(message: string) {
    this._snackBar.open(message, "DISMISS");
  }
}
