import { Component, OnInit } from '@angular/core';
import { OAuthService, OAuthErrorEvent, UserInfo } from 'angular-oauth2-oidc';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(
    private oauthService: OAuthService,
    private router: Router

  ) {
  }

  ngOnInit(): void {
    const token = this.oauthService.getAccessToken();
    const cliams = this.oauthService.getIdentityClaims();
    if (token && cliams) {
      this.router.navigateByUrl('/index');
    }

    this.oauthService.events.subscribe(event => {
      if (event instanceof OAuthErrorEvent) {
        // TODO:处理错误
        console.error(event);
      } else {
        if (event.type === 'token_received' || event.type === 'token_refreshed') {
          this.oauthService.loadUserProfile()
            .then(() => {
              this.router.navigateByUrl('/index');
            });
        }
      }
    });
  }

  login(): void {
    this.oauthService.initCodeFlow();
  }

  logout(): void {
    this.oauthService.logOut();
  }

  get userName(): string | null {
    const claims: any = this.oauthService.getIdentityClaims();
    if (!claims) { return null; }
    return claims.given_name;
  }

}
