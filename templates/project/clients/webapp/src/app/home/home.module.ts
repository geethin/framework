import { NgModule } from '@angular/core';
import { HomeRoutingModule } from './home-routing.module';
import { LoginComponent } from './login/login.component';
import { ShareModule } from '../share/share.module';


@NgModule({
  declarations: [LoginComponent],
  imports: [
    ShareModule,
    HomeRoutingModule
  ]
})
export class HomeModule { }
