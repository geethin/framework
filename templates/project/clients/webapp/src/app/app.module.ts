import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { ShareModule } from './share/share.module';
import { OAuthModule } from 'angular-oauth2-oidc';
import { HomeModule } from './home/home.module';

const appModules = [
  HomeModule
];

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    OAuthModule.forRoot({
      resourceServer: {
        allowedUrls: ['https://localhost:5001'],
        sendAccessToken: true
      }
    }),
    ShareModule,
    ...appModules
  ],
  providers: [

  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
