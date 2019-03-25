import { Injectable } from '@angular/core';

export interface Credentials {
  // Customize received credentials here
  userName: string;
  name: string;
  caveat: string;
  token: string;
  roles: string[];
}

export const credentialsKey = 'credentials';

/**
 * Provides access to credentials without the extra weight of the HttpClient injection
 */
@Injectable()
export class AuthorizationService {

  private _credentials: Credentials | null;
  private _isRemembered: boolean;

  constructor() {
    let storedCredentials = localStorage.getItem(credentialsKey);
    let tempCredentials = sessionStorage.getItem(credentialsKey);

    if (storedCredentials)
      this._isRemembered = true;

    const savedCredentials = this._isRemembered ? storedCredentials : tempCredentials;
    if (savedCredentials) {
      this._credentials = JSON.parse(savedCredentials);
    }
  }

  /**
   * Checks is the user is authenticated.
   * @return True if the user is authenticated.
   */
  isAuthenticated(): boolean {
    return !!this.credentials;
  }

  isInRole(roleName: string): boolean {
    if (!this._credentials.roles)
      return false;

    return this._credentials.roles.some((userRole) => userRole === roleName);
  }

  /**
   * Gets the user credentials.
   * @return The user credentials or null if the user is not authenticated.
   */
  get credentials(): Credentials | null {
    return this._credentials;
  }

  get isRemembered(): boolean {
    return this._isRemembered;
  }

  /**
   * Sets the user credentials.
   * The credentials may be persisted across sessions by setting the `remember` parameter to true.
   * Otherwise, the credentials are only persisted for the current session.
   * @param credentials The user credentials.
   * @param remember True to remember credentials across sessions.
   */
  public setCredentials(credentials?: Credentials, remember?: boolean) {
    this._credentials = credentials || null;
    this._isRemembered = remember;

    console.log(JSON.stringify(credentials));

    if (credentials) {
      const storage = remember ? localStorage : sessionStorage;
      storage.setItem(credentialsKey, JSON.stringify(credentials));
    } else {
      sessionStorage.removeItem(credentialsKey);
      localStorage.removeItem(credentialsKey);
    }
  }

  public clearCredentials() {
    this.setCredentials();
  }

  public clearCaveat() {
    this.credentials.caveat = null;
    this.setCredentials(this.credentials, this.isRemembered);
  }
}
