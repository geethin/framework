import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css']
})
export class LayoutComponent implements OnInit {
  isLogin = false;
  username = '';
  constructor(
    private service: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.isLogin = this.service.isLogin;
    this.username = localStorage.getItem('username')!;
  }

  logout() {
    this.service.logout();
    this.router.navigateByUrl('/login');
  }

}
