import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { DatepickerConfig, BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { User } from '../models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  user: User;
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup;
  datepickerConfig: Partial<BsDatepickerConfig>;

  constructor(private authService: AuthService, private router: Router, private alertifyService: AlertifyService, private formBuilder: FormBuilder) { }

  ngOnInit() {
    this.datepickerConfig = {
      containerClass: 'theme-red',
      maxDate: new Date()
    };
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.formBuilder.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', Validators.required]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(group: FormGroup) {
    return group.get('password').value === group.get('confirmPassword').value ? null : { mismatch: true };
  }

  register() {
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);
      this.authService.register(this.user).subscribe(() => {
        this.alertifyService.success('Registration successfull');
      }, error => {
        this.alertifyService.error(error);
      }, () => {
        this.authService.login(this.user).subscribe(next => {
          this.router.navigate(['/members']);
        }, error => {
          this.alertifyService.error(error);
        });
      });
    }



  }
  cancel() {
    this.cancelRegister.emit(false);
    console.log('cancelled');

  }

}
