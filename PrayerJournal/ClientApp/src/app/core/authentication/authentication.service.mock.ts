import { Observable, of } from 'rxjs';

import { LoginContext } from './authentication.service';
import { Credentials } from '../authorization.service';

export class MockAuthenticationService {

  credentials: Credentials | null = {
    username: 'test',
    caveat: null,
    token: '123'
  };

  login(context: LoginContext): Observable<Credentials> {
    return of({
      username: context.username,
      caveat: null,
      token: '123456'
    });
  }

  logout(): Observable<boolean> {
    this.credentials = null;
    return of(true);
  }

  isAuthenticated(): boolean {
    return !!this.credentials;
  }

}
