import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { SignupComponent } from './components/signup/signup.component';
import { HttpClientModule } from '@angular/common/http';
import { MainpageComponent } from './components/mainpage/mainpage.component';
import { NgToastModule, NgToastService } from 'ng-angular-popup';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    SignupComponent,
    MainpageComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    HttpClientModule,
    NgToastModule,
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
