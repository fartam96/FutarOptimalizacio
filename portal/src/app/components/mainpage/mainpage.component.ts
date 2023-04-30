import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import { UserStoreService } from 'src/app/services/user-store.service';

@Component({
  selector: 'app-mainpage',
  templateUrl: './mainpage.component.html',
  styleUrls: ['./mainpage.component.scss'],
})
export class MainpageComponent implements OnInit {
  public users: any = [];
  public role!: string;
  public nearestList: any = [];
  public cityList: any = [];
  public bruteFResponse: any = [];
  public vrpNNResponse: any = [];
  public vrpRandomResponse: any = [];
  public unique_name: string = '';
  public showTable: boolean = false;
  public showNN: boolean = false;

  cityNumber!: FormGroup;

  cityAndCourNumber!: FormGroup;

  cityAndCourRandom!: FormGroup;

  constructor(
    private api: ApiService,
    private auth: AuthService,
    private fb: FormBuilder,
    private userStore: UserStoreService
  ) {}

  ngOnInit() {
    this.api.getUsers().subscribe((res) => {
      this.users = res;
    });

    this.cityAndCourNumber = this.fb.group({
      cityNumber: ['', Validators.required],
      courNumber: ['', Validators.required],
    });

    this.cityNumber = this.fb.group({
      cityNum: ['', Validators.required],
    });

    this.cityAndCourRandom = this.fb.group({
      cityNumberR: ['', Validators.required],
      courNumberR: ['', Validators.required],
    });

    this.userStore.getFullNameFromStore().subscribe((val) => {
      const fullNameFromToken = this.auth.getfullNameFromToken();
      this.unique_name = val || fullNameFromToken;
    });

    this.userStore.getRoleFromStore().subscribe((val) => {
      const roleFromToken = this.auth.getRoleFromToken();
      this.role = val || roleFromToken;
    });
  }

  logout() {
    this.auth.signOut();
  }

  nearestNTsp() {
    this.api.getNNNTsp().subscribe((res) => {
      this.nearestList = res;
    });
  }

  toggleShowTable(): void {
    this.showTable = !this.showTable;
  }

  toggleNNTable(): void {
    this.showNN = !this.showNN;
  }

  showNNTable() {
    this.nearestNTsp();
    this.toggleNNTable();
  }

  showBruteWithDiffCities() {
    this.api
      .getBruteForce(Number(this.cityNumber.controls.cityNum.value))
      .subscribe((res) => {
        this.bruteFResponse = res;
        this.cityList = res.cities;
        console.log(this.cityList);
      });
    console.log(this.bruteFResponse);
  }

  VRPNNclosest() {
    this.api
      .getVRPNN(
        Number(this.cityAndCourNumber.controls.cityNumber.value),
        Number(this.cityAndCourNumber.controls.courNumber.value)
      )
      .subscribe((res) => {
        this.vrpNNResponse = res;
        console.log(this.vrpNNResponse);
      });
  }

  VRPNNRandom() {
    this.api
      .getVRPRandom(
        Number(this.cityAndCourRandom.controls.cityNumberR.value),
        Number(this.cityAndCourRandom.controls.courNumberR.value)
      )
      .subscribe((res) => {
        this.vrpRandomResponse = res;
        console.log(this.vrpNNResponse);
      });
  }
}
