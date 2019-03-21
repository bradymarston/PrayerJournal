import { Observable, of } from 'rxjs';

import { LoginContext } from './authentication.service';
import { Credentials } from '../authorization.service';

export class MockAuthenticationService {

  credentials: Credentials | null = {
    userName: 'test',
    name: "Test",
    caveat: null,
    token: '123'
  };

  login(context: LoginContext): Observable<Credentials> {
    return of({
      userName: context.email,
      name: "",
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
