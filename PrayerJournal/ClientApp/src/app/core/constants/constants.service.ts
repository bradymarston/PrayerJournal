import { Injectable } from '@angular/core';

export interface ExternalLoginProvider {
  id: string,
  displayName: string,
  imageUrl: string,
  codeUrl: string,
  clientId: string,
  otherQueryParams: string
}

@Injectable()
export class ConstantsService {

  private _externalLoginProviders: ExternalLoginProvider[] = [
    {
      id: "Google",
      displayName: "Google",
      imageUrl: "assets/google-logo-72.png",
      codeUrl: "https://accounts.google.com/o/oauth2/v2/auth",
      clientId: "350476418062-0me9iljbrpb9tva5kh97ddppv53i3kgf.apps.googleusercontent.com",
      otherQueryParams: "&response_type=code&scope=profile"
    },
    {
      id: "Microsoft",
      displayName: "Microsoft",
      imageUrl: "assets/ms-symbollockup_mssymbol_19.png",
      codeUrl: "//login.microsoftonline.com/common/oauth2/v2.0/authorize",
      clientId: "f3619043-88c9-419d-8167-7a42abb86ae3",
      otherQueryParams: "&response_type=code&scope=https%3A%2F%2Fgraph.microsoft.com%2Fuser.read"
    },
    {
      id: "Facebook",
      displayName: "Facebook",
      imageUrl: "assets/f-ogo_RGB_HEX-58.png",
      codeUrl: "https://www.facebook.com/v3.2/dialog/oauth",
      clientId: "2261817204105691",
      otherQueryParams: ""
    }
  ]

  getExternalLoginProvider(providerId: string) {
    return this.externalLoginProviders.find(p => p.id === providerId);
  }

  get externalLoginProviders() {
    return this._externalLoginProviders;
  }
}
