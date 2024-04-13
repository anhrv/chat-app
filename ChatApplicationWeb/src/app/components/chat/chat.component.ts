import {AfterViewChecked, Component, ElementRef, inject, OnInit, ViewChild} from '@angular/core';
import {ChatService} from "../../services/chat.service";
import {FormsModule} from "@angular/forms";
import {Router} from "@angular/router";
import {AsyncPipe, DatePipe, NgClass, NgForOf, NgIf} from "@angular/common";
@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [
    FormsModule,
    NgClass,
    DatePipe,
    AsyncPipe,
    NgForOf,
    NgIf
  ],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css'
})
export class ChatComponent implements OnInit, AfterViewChecked {

  chatService = inject(ChatService);
  router = inject(Router);
  inputMessage = "";
  messages: any[] = [];
  loggedInUserName = sessionStorage.getItem("user");
  roomName = sessionStorage.getItem("room");
  @ViewChild("scrollMe") private scrollContainer!:ElementRef;

  ngOnInit(): void {
    this.chatService.messages$.subscribe(res => {
      this.messages = res;
    });
  }

  ngAfterViewChecked(): void {
    this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
  }

  sendMessage(){
    this.chatService.sendMessage(this.inputMessage)
      .then(()=>{
        this.inputMessage = '';
      }).catch((err)=>{
      console.log(err);
    })
  }

  leaveChat() {
    this.chatService.leaveChat()
      .then(() => {
        this.router.navigate(['welcome']);
        setTimeout(() => {
          location.reload();
        }, 0);
      }).catch((err) => {
      console.log(err);
    })
  }

}
