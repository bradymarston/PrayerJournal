import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material';
import { ConfirmDialogOptions, ConfirmDialogComponent } from '../dialogs/confirm-dialog/confirm-dialog.component';
import { Observable } from 'rxjs';

@Injectable()
export class DialogService {
  constructor(private _dialog: MatDialog) { }

  confirm(options: ConfirmDialogOptions): Observable<boolean> {
    return this._dialog.open(ConfirmDialogComponent, {
      autoFocus: false,
      restoreFocus: false,
      data: options
    }).afterClosed();
  }
}
