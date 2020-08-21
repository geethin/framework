import { Component, OnInit } from '@angular/core';
import { {$EntityName}Service } from 'src/app/services/{$EntityPathName}.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { {$EntityName}Dto } from 'src/app/share/models/{$EntityPathName}-dto.model';

@Component({
  selector: 'app-detail',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.css']
})
export class DetailComponent implements OnInit {
  id: string = null;
  isLoading = true;
  data = {} as {$EntityName}Dto;
  constructor(
    private service: {$EntityName}Service,
    private snb: MatSnackBar,
    private route: ActivatedRoute,
  ) {
    this.id = this.route.snapshot.paramMap.get('id');
  }
  ngOnInit(): void {
    this.getDetail();
  }
  getDetail(): void {
    this.service.getDetail(this.id)
      .subscribe(res => {
        this.data = res;
        this.isLoading = false;
      }, error => {
        this.snb.open(error);
      })
  }

}
