import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-mainpage',
  templateUrl: './mainpage.component.html',
  styleUrls: ['./mainpage.component.scss'],
})
export class MainpageComponent implements OnInit {
  public users: any = [];
  constructor(private api: ApiService, private auth: AuthService) {}

  ngOnInit(): void {
    this.api.getUsers().subscribe((res) => {
      this.users = res;
    });
  }

  logout() {
    this.auth.signOut();
  }
}
