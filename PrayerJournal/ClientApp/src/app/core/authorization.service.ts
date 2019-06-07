import { Injectable } from '@angular/core';

export interface StoredUserInfo {
  name: string;
  hasPassword: boolean;
  caveat: string;
  roles: string[];
}

export interface StoredAuthInfo {
  userId: string;
  token: string;
}

export const userInfoKey = 'user-info';
export const authInfoKey = 'auth-info';

export const roleIdentifiers = [
  "Admin",
  "Reviewer",
  "AddUsers"
];

export const roleNames = {
  "Admin": "Administrator",
  "Reviewer": "Reviewer",
  "AddUsers": "User Entry"
};

/**
 * Provides access to credentials without the extra weight of the HttpClient injection
 */
@Injectable()
export class AuthorizationService {

  private _userInfo: StoredUserInfo | null;
  private _authInfo: StoredAuthInfo | null;
  private _isRemembered: boolean;

  constructor() {
    let localUserInfo = localStorage.getItem(userInfoKey);
    let sessionUserInfo = sessionStorage.getItem(userInfoKey);
    let localAuthInfo = localStorage.getItem(authInfoKey);
    let sessionAuthInfo = sessionStorage.getItem(authInfoKey);

    if (localUserInfo && localUserInfo)
      this._isRemembered = true;
    else {
      if (localUserInfo)
        sessionUserInfo = localUserInfo;

      if (localAuthInfo)
        sessionAuthInfo = localAuthInfo;
    }

    if (this, this._isRemembered) {
      this._userInfo = JSON.parse(localUserInfo);
      this._authInfo = JSON.parse(localAuthInfo);
    }
    else
      if (sessionUserInfo && sessionAuthInfo) {
        this._userInfo = JSON.parse(sessionUserInfo);
        this._authInfo = JSON.parse(sessionAuthInfo);
      }

    this.syncStoredCredentials();
  }

  /**
   * Checks is the user is authenticated.
   * @return True if the user is authenticated.
   */
  isAuthenticated(): boolean {
    return (!!this.userInfo && !!this.authInfo);
  }

  isInRole(roleName: string): boolean {
    if (!this.userInfo.roles)
      return false;

    return this.userInfo.roles.includes(roleName);
  }

  isCurrentUser(userId: string): boolean {
    return this.authInfo.userId === userId;
  }

  setRoles(roles: string[]) {
    this.userInfo.roles = roles;
    this.syncStoredCredentials();
  }

  private syncStoredCredentials() {
    this.setUserInfo(this.userInfo, this.isRemembered);
    this.setAuthInfo(this.authInfo, this.isRemembered);
  }

  /**
   * Gets the user information.
   * @return The user info or null if the user is not authenticated.
   */
  get userInfo(): StoredUserInfo | null {
    return this._userInfo;
  }

  /**
  * Gets the auth information.
  * @return The auth info or null if the user is not authenticated.
  */
  get authInfo(): StoredAuthInfo | null {
    return this._authInfo;
  }

  get isRemembered(): boolean {
    return this._isRemembered;
  }

  /**
   * Sets the user info.
   * The credentials may be persisted across sessions by setting the `remember` parameter to true.
   * Otherwise, the credentials are only persisted for the current session.
   * @param credentials The user credentials.
   * @param remember True to remember credentials across sessions.
   */
  public setUserInfo(userInfo?: StoredUserInfo, remember?: boolean) {
    this._userInfo = userInfo || null;

    if (userInfo) {
      const storage = remember ? localStorage : sessionStorage;
      const storageToClear = !remember ? localStorage : sessionStorage;
      storage.setItem(userInfoKey, JSON.stringify(userInfo));
      storageToClear.removeItem(userInfoKey);
      //Move authInfo is remember has changed
      if (this.isRemembered != remember && this.authInfo) {
        storage.setItem(authInfoKey, JSON.stringify(this.authInfo));
        storageToClear.removeItem(authInfoKey);
      }
    } else {
      sessionStorage.removeItem(userInfoKey);
      localStorage.removeItem(userInfoKey);
    }

    this._isRemembered = remember;
  }

/**
 * Sets the auth info.
 * The credentials may be persisted across sessions by setting the `remember` parameter to true.
 * Otherwise, the credentials are only persisted for the current session.
 * @param credentials The user credentials.
 * @param remember True to remember credentials across sessions.
 */
  public setAuthInfo(authInfo?: StoredAuthInfo, remember?: boolean) {
    this._authInfo = authInfo || null;

    if (authInfo) {
      const storage = remember ? localStorage : sessionStorage;
      const storageToClear = !remember ? localStorage : sessionStorage;
      storage.setItem(authInfoKey, JSON.stringify(authInfo));
      storageToClear.removeItem(authInfoKey);
      //Move userInfo is remember has changed
      if (this.isRemembered != remember && this.userInfo) {
        storage.setItem(userInfoKey, JSON.stringify(this.userInfo));
        storageToClear.removeItem(userInfoKey);
      }
    } else {
      sessionStorage.removeItem(authInfoKey);
      localStorage.removeItem(authInfoKey);
    }

    this._isRemembered = remember;
  }

  public clearCredentials() {
    this.setUserInfo();
    this.setAuthInfo();
  }

  public clearCaveat() {
    this.userInfo.caveat = null;
    this.syncStoredCredentials();
  }
}
