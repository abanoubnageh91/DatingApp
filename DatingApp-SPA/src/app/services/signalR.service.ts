import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { environment } from 'src/environments/environment';
@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  constructor() { }

  private hubConnection: signalR.HubConnection
  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}message`, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .build();
    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err))
  }

  public addTransferChartDataListener = (next) => {
    this.hubConnection.on('transfermessagedata', (data) => {
      // this.data = data;
      // console.log(data);
      next();
    });
  }

}
