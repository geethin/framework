import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { {$EntityName}Service } from '../../services/{$EntityPathName}.service';
import { {$EntityName}AddDto } from '../../share/models/{$EntityPathName}-add-dto.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { Status } from 'src/app/share/models/status.model';

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.css']
})
export class AddComponent implements OnInit {

  formGroup: FormGroup;
  data = {} as {$EntityName}AddDto;
  isLoading = true;
  status = Status;
  constructor(
    private service: {$EntityName}Service,
    public snb: MatSnackBar,
    private router: Router
  ) {

  }

{$DefinedProperties}

  ngOnInit(): void {
    this.initForm();
    // TODO:获取其他相关数据
  }

  initForm(): void {
    this.formGroup = new FormGroup({
{$DefinedFormControls}
    });
  }
  getValidatorMessage(type: string): string {
    switch (type) {
{$DefinedValidatorMessage}
      default:
        return '';
    }
  }

  add(): void {
    if (this.formGroup.valid) {
      this.data = this.formGroup.value as {$EntityName}AddDto;
      this.service.add(this.data)
        .subscribe(res => {
          this.snb.open('添加成功');
          // this.router.navigateByUrl('/{$EntityPathName}/index');
        });
    }
  }
}