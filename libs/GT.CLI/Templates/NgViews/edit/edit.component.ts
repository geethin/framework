import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { {$EntityName}Dto } from 'src/app/share/models/{$EntityPathName}-dto.model';
import { {$EntityName}Service } from 'src/app/services/{$EntityPathName}.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { {$EntityName}UpdateDto } from 'src/app/share/models/{$EntityPathName}-update-dto.model';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.css']
})
export class EditComponent implements OnInit {
  id: string = null;
  isLoading = true;
  data = {} as {$EntityName}Dto;
  updateData = {} as {$EntityName}UpdateDto;
  formGroup: FormGroup;
  constructor(
    private service: {$EntityName}Service,
    private snb: MatSnackBar,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.id = this.route.snapshot.paramMap.get('id');
  }

{$DefinedProperties}

  ngOnInit(): void {
    this.getDetail();
  }

  getDetail(): void {
    this.service.getDetail(this.id)
      .subscribe(res => {
        this.data = res;
        this.initForm();
        this.isLoading = false;
      }, error => {
        this.snb.open(error);
      })
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
  edit(): void {
    if (this.formGroup.valid) {
      this.updateData = this.formGroup.value as {$EntityName}UpdateDto;
      this.service.update(this.id, this.updateData)
        .subscribe(res => {
          this.snb.open('修改成功');
          // this.router.navigateByUrl('/{$EntityPathName}/index');
        });
    }
  }
}
