import { Component } from '@angular/core';
import { OAuthService, AuthConfig } from 'angular-oauth2-oidc';

export const authCodeFlowConfig: AuthConfig = {
  // Url of the Identity Provider
  issuer: 'https://localhost:5001',
  redirectUri: window.location.origin + '/index.html',
  clientId: 'webapp',
  // Just needed if your auth server demands a secret. In general, this
  // is a sign that the auth server is not configured with SPAs in mind
  // and it might not enforce further best practices vital for security
  // such applications.
  // dummyClientSecret: 'secret',
  responseType: 'code',
  scope: 'openid profile webapp offline_access',
  showDebugInformation: true,
};



@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'webapp';
  constructor(private oauthService: OAuthService) {

    this.oauthService.configure(authCodeFlowConfig);
    this.oauthService.loadDiscoveryDocumentAndTryLogin();
  }
}
