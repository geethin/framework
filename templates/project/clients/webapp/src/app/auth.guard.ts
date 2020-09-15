import { Injectable } from '@angular/core';
import { CanActivate, CanActivateChild, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate, CanActivateChild {
    constructor(
        private router: Router,
        private oauthService: OAuthService,
    ) { }

    canActivate(
        next: ActivatedRouteSnapshot,
        state: RouterStateSnapshot): boolean {

        const isLogin = this.oauthService.hasValidAccessToken();
        const url = state.url;

        if (url.startsWith('/login')) {
            return true;
        }
        if (!isLogin) {
            this.router.navigate(['/login']);
            return false;
        }
        return true;
    }
    canActivateChild(
        next: ActivatedRouteSnapshot,
        state: RouterStateSnapshot): boolean {
        return this.canActivate(next, state);
    }

}
