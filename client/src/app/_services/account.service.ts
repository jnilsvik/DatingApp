import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import {ReplaySubject} from "rxjs";
import {User} from "../_models/user";
// this means that it can be injected in other components or services
@Injectable({
  //
  providedIn: 'root'
})
export class AccountService {
  baseURL = 'https://localhost:5001/api/'
  private currentUserSrc = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSrc.asObservable();

  constructor(private http: HttpClient) { }
  // methtod to handle login
  login(model:any) {
    return this.http.post(this.baseURL + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
         localStorage.setItem("user", JSON.stringify(user));
         this.currentUserSrc.next(user)
        }
      })
    )
  }

  setCurrentUser(user: User){
    this.currentUserSrc.next(user)
  }

  logout(){
    localStorage.removeItem("user")
    this.currentUserSrc.next(null)
  }
}
