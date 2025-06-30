import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { signal } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class StationWorkSignalRService {
  private hubConnection!: signalR.HubConnection;
  public trigger = signal(false);
  private baseUrl = environment.apiBaseUrl;
  constructor() {
    this.startConnection();
  }

  private startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      //.withUrl(this.baseUrl +"/stationWorkHub")
      .withUrl('http://localhost:5143/stationWorkHub')
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR Connected'))
      .catch(err => console.error('SignalR Error:', err));

    this.hubConnection.on('ReceiveStationWorkUpdate', () => {
      this.trigger.set(true); // dispara actualización
    });
  }
}
