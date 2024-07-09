import { Component, HostListener, inject, OnInit, ViewChild, viewChild } from '@angular/core';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';
import { Member } from '../../_models/member';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { PhotoEditComponent } from "../photo-edit/photo-edit.component";
import { every } from 'rxjs';

@Component({
  selector: 'app-member-edit',
  standalone: true,
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css',
  imports: [TabsModule, FormsModule, ToastrModule, PhotoEditComponent]
})
export class MemberEditComponent implements OnInit {

  @ViewChild('editForm') editForm?: NgForm;
  @HostListener('window:beforeunload', ['$event']) notify($event: any) {
    if (this.editForm?.dirty) {
      $event.returnValue = true;
    }
  }
  member?: Member;

  private accountService = inject(AccountService);
  private memberService = inject(MembersService);
  private toastr = inject(ToastrService);


  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    const user = this.accountService.currentUser();
    if (!user) return;
    this.memberService.getMember(user.username).subscribe({
      next: member => this.member = member
    })
  }

  updateMember() {
    this.memberService.updateMember(this.editForm?.value).subscribe({
      next: _ => {
        this.toastr.success('Profile Updated Successfully');
        this.editForm?.reset(this.member);
      }
    })

  }

  onMemberChange(event: Member) {
    this.member = event;
  }
}
