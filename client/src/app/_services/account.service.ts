import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
// this means that it can be injected in other components or services
@Injectable({
  //
  providedIn: 'root'
})
export class AccountService {
  baseURL = 'https://localhost:5001/api/'

  constructor(private http: HttpClient) { }
  // methtod to handle login
  login(model:any) {
    return this.http.post(this.baseURL + 'account/login', model)
  }
}
