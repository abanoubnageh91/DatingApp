import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user';
import { Message } from '../models/message';
import { PaginatedResult } from '../models/pagination';
import { map } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl: string = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getUsers(pageNumber?, pageSize?, userParams?, likesParam?): Observable<PaginatedResult<User[]>> {
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();



    let params = new HttpParams();
    if (pageNumber != null && pageSize != null) {
      params = new HttpParams(
        {
          fromObject: { pageNumber, pageSize }
        }
      );
    }
    if (userParams != null) {
      params = new HttpParams(
        {
          fromObject: { pageNumber, pageSize, minAge: userParams.minAge, maxAge: userParams.maxAge, gender: userParams.gender, orderBy: userParams.orderBy }
        }
      );
    }
    if (likesParam === 'Likers') {
      params = new HttpParams(
        {
          fromObject: { pageNumber, pageSize, likers: 'true' }
        }
      );
    }

    if (likesParam === 'Likees') {
      params = new HttpParams(
        {
          fromObject: { pageNumber, pageSize, likees: 'true' }
        }
      );
    }

    return this.http.get<User[]>(this.baseUrl + 'users', {
      observe: 'response',
      params
    }).pipe(
      map((response) => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') != null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
  }

  getUser(id: number): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + 'users/' + id, user);
  }

  setMainPhoto(userId: number, id: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setmain', {});
  }

  deletePhoto(userId: number, id: number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
  }

  sendLike(id: number, recipientId: number) {
    return this.http.post(this.baseUrl + 'users/' + id + '/like/' + recipientId, {});
  }

  getMessages(userid, pageNumber?, pageSize?, messageContainer?): Observable<PaginatedResult<Message[]>> {
    const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();



    let params = new HttpParams();
    if (pageNumber != null && pageSize != null) {
      params = new HttpParams(
        {
          fromObject: { pageNumber, pageSize }
        }
      );
    }
    if (messageContainer != null) {
      params = new HttpParams(
        {
          fromObject: { pageNumber, pageSize, messageContainer }
        }
      );
    }

    return this.http.get<Message[]>(this.baseUrl + 'users/' + userid + '/messages', {
      observe: 'response',
      params
    }).pipe(
      map((response) => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') != null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
  }

  getMessageThread(id: number, recipientId: number) {
    return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages/thread/' + recipientId);
  }

  sendMessage(id: number, message: Message) {
    return this.http.post(this.baseUrl + 'users/' + id + '/messages', message);
  }

  deleteMessage(userId: number, id: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + id, {});
  }

  markAsRead(userId: number, id: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + id + '/read', {}).subscribe();
  }
}
