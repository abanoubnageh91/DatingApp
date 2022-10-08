import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/models/message';
import { AuthService } from 'src/app/services/auth.service';
import { AlertifyService } from 'src/app/services/alertify.service';
import { UserService } from 'src/app/services/user.service';
import { tap } from 'rxjs/operators';
import {SignalRService} from 'src/app/services/signalR.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() recipientId: number;
  messages: Message[];
  newMessage: any = {};
  constructor(private userService: UserService, private authService: AuthService, private alertifyService: AlertifyService,
    public signalRService: SignalRService) { }

  ngOnInit() {
    this.loadMessages();
    this.signalRService.startConnection();
    this.signalRService.addTransferChartDataListener(()=>{
      this.loadMessages();
    });
  }

  loadMessages() {
    const currentUserId = +this.authService.decodedToken.nameid;
    this.userService.getMessageThread(this.authService.decodedToken.nameid, this.recipientId)
      .pipe(tap(messages => {
        for (let i = 0; i < messages.length; i++) {
          if (!messages[i].isRead && messages[i].recipientId === currentUserId) {
            this.userService.markAsRead(currentUserId, messages[i].id);
          }

        }
      }))
      .subscribe(res => {
        this.messages = res;
      }, error => {
        this.alertifyService.error(error);
      });
  }

  sendMessage() {
    this.newMessage.recipientId = this.recipientId;
    this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage)
      .subscribe((message: Message) => {
        this.messages.unshift(message);
        this.newMessage.content = '';
      }, error => {
        this.alertifyService.error(error);
      })
  }

}
