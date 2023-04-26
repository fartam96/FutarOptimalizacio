import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private baseUrl: string = 'https://localhost:7148/api/User/';
  private tspAlgUrl: string = 'https://localhost:7148/api/TSPalg/';
  private bruteUrl: string = 'https://localhost:7148/api/TSPalg/TSPBruteForce';

  constructor(private http: HttpClient) {}

  getUsers() {
    return this.http.get<any>(this.baseUrl);
  }

  getNNNTsp() {
    return this.http.get<any>(`${this.tspAlgUrl}nearestNeighbor`);
  }

  getBruteForce(cityNumber: any) {
    return this.http.post<any>(`${this.bruteUrl}`, cityNumber);
  }
}
