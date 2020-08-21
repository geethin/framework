import { Component, OnInit, ViewChild } from '@angular/core';
import { {$EntityName}Service } from 'src/app/services/{$EntityPathName}.service';
import { Router } from '@angular/router';
import { ConfirmDialogComponent } from 'src/app/share/confirm-dialog/confirm-dialog.component';
import { {$EntityName}Dto } from '../../share/models/{$EntityPathName}-dto.model';
import { {$EntityName}Filter } from '../../share/models/{$EntityPathName}-filter.model';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.css']
})
export class IndexComponent implements OnInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  isLoading = true;
  data: {$EntityName}Dto[] = [];
  columns: string[] = [{$Columns}];
  dataSource: MatTableDataSource<{$EntityName}Dto>;
  filter: {$EntityName}Filter;
  pageSizeOption = [10, 20, 50];
  constructor(
    private service: {$EntityName}Service,
    private snb: MatSnackBar,
    private dialog: MatDialog,
    private router: Router,
  ) {

    this.filter = {
      pageIndex: 1,
      pageSize: 12
    }
  }

  ngOnInit() {
    this.getList();
  }

  getList(): void {
    this.service.filter(this.filter)
      .subscribe(res => {
        this.data = res.data;
        this.dataSource = new MatTableDataSource<{$EntityName}Dto>(this.data);
      });
  }

  deleteConfirm(item: {$EntityName}Dto): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      hasBackdrop: true,
      disableClose: false,
      data: {
        title: '删除',
        content: '是否确定删除?'
      }
    });

    ref.afterClosed().subscribe(res => {
      if (res) {
        this.delete(item);
      }
    });
  }
  delete(item: {$EntityName}Dto): void {
    this.service.delete(item.id)
      .subscribe(res => {
        this.data = this.data.filter(_ => _.id !== res.id);
        this.dataSource.data = this.data;
        this.snb.open('删除成功');
      });
  }
  /**
   * 编辑
   */
  edit(id: string): void {
    console.log(id);
    this.router.navigateByUrl('/{$EntityPathName}/edit/' + id);
  }

}
