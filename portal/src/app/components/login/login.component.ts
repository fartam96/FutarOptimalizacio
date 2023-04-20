import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import ValidateForm from 'src/app/helpers/validateform';
import { AuthService } from 'src/app/services/auth.service';
import { ResetPassService } from 'src/app/services/reset-pass.service';
import { UserStoreService } from 'src/app/services/user-store.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  type: string = 'password';
  isText: boolean = false;
  eyeIcon: string = 'fa-eye-slash';
  public resetPasswordEmail!: string;
  public isValidEmail!: boolean;

  loginForm!: FormGroup;

  constructor(
    private resetService: ResetPassService,
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private toast: NgToastService,
    private userStore: UserStoreService
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  hideShowPass() {
    this.isText = !this.isText;
    this.isText ? (this.eyeIcon = 'fa-eye') : (this.eyeIcon = 'fa-eye-slash');
    this.isText ? (this.type = 'text') : (this.type = 'password');
  }

  onLogin() {
    if (this.loginForm.valid) {
      console.log(this.loginForm.value);
      this.auth.login(this.loginForm.value).subscribe({
        next: (res) => {
          console.log(res.message);
          this.loginForm.reset();
          this.auth.storeToken(res.token);
          const tokenPayload = this.auth.decodedToken();
          this.userStore.setFullNameForStore(tokenPayload.unique_name);
          this.userStore.setRoleForStore(tokenPayload.role);
          this.toast.success({
            detail: 'SUCCESS',
            summary: res.message,
            duration: 5000,
          });
          this.router.navigate(['mainpage']);
        },
        error: (err) => {
          this.toast.error({
            detail: 'ERROR',
            summary: 'Something when wrong!',
            duration: 5000,
          });
          console.log(err);
        },
      });
    } else {
      ValidateForm.validateAllFormFields(this.loginForm);
    }
  }

  checkValidEmail(event: string) {
    const value = event;
    const pattern = /^[a-z0-9]+@[a-z]+\.[a-z]{2,3}$/;
    this.isValidEmail = pattern.test(value);
    return this.isValidEmail;
  }

  confirmToSend() {
    if (this.checkValidEmail(this.resetPasswordEmail))
      console.log(this.resetPasswordEmail);

    this.resetService.sendResetPasswordLink(this.resetPasswordEmail).subscribe({
      next: (res) => {
        this.toast.success({
          detail: 'Succes',
          summary: 'Reset is succesfull',
          duration: 3000,
        });
        this.resetPasswordEmail = '';
        const buttonRef = document.getElementById('closeBtn');
        buttonRef?.click();
      },
      error: (err) => {
        this.toast.error({
          detail: 'Error',
          summary: 'Failed to reset pass',
          duration: 3000,
        });
      },
    });
  }
}
