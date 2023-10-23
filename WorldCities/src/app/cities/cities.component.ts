//import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { environment } from '../../environments/environment.prod';
import { MatSort } from '@angular/material/sort';
import { City } from './city';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { CityService } from './city.service';

@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrls: ['./cities.component.scss']
})
export class CitiesComponent implements OnInit {
  public displayedColumns: string[] =
    ['id', 'name', 'lat', 'lon', 'countryName'];
  public cities!: MatTableDataSource<City>;

  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;
  public defaultSortColumn: string = "name";
  public defaultSortOrder: "asc" | "desc" = "asc";

  defaultFilterColumn: string = "name";
  filterQuery?: string;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  filterTextChanged: Subject<string> = new Subject<string>();

  constructor(private cityService: CityService) {
  }

  ngOnInit(): void {
    this.loadData();
  }

  //debounce filter text changes
  //to reduce http requests to server, and increase performance
  onFilterChanged(filterText: string) {
    if (this.filterTextChanged.observers.length === 0) {
      this.filterTextChanged
        .pipe(debounceTime(1000), distinctUntilChanged())
        .subscribe(query => {
          this.loadData(query);
        });
    }
    this.filterTextChanged.next(filterText);
  }

  loadData(query?: string): void {
    var pageEvent = new PageEvent();
    pageEvent.pageIndex = this.defaultPageIndex;
    pageEvent.pageSize = this.defaultPageSize;
    this.filterQuery = query;
    this.getData(pageEvent);
  }
  //ToDo: check why sort is nukk?!! when double sort
  getData(event: PageEvent): void {
    var sortColumn = (this.sort)
      ? this.sort.active
      : this.defaultSortColumn;

    var sortOrder = (this.sort)
      ? this.sort.direction
      : this.defaultSortOrder;

    var filterColumn = (this.filterQuery)
      ? this.defaultFilterColumn
      : null;

    var filterQuery = this.filterQuery
      ? this.filterQuery
      : null;


    this.cityService.getData(
      event.pageIndex,
      event.pageSize,
      sortColumn,
      sortOrder,
      filterColumn,
      filterQuery)
      .subscribe(result => {
        this.paginator.length = result.totalCount;
        this.paginator.pageIndex = result.pageIndex;
        this.paginator.pageSize = result.pageSize;
        this.cities = new MatTableDataSource<City>(result.data);

      }, error => console.error(error));

    //var url = environment.baseUrl + 'api/Cities';

    //var params = new HttpParams()
    //  .set("pageIndex", event.pageIndex.toString())
    //  .set("pageSize", event.pageSize.toString())
    //  .set("sortColumn", (this.sort)
    //    ? this.sort.active
    //    : this.defaultSortColumn)
    //  .set("sortOrder", (this.sort)
    //    ? this.sort.direction
    //    : this.defaultSortOrder);

    ////filtering
    //if (this.filterQuery) {
    //  params = params
    //    .set("filterColumn", this.defaultFilterColumn)
    //    .set("filterQuery", this.filterQuery);
    //}
  }
}

//    this.http.get<any>(url, { params })
//      .subscribe(result => {
//        console.log(result);
//        this.paginator.length = result.totalCount;
//        this.paginator.pageIndex = result.pageIndex;
//        this.paginator.pageSize = result.pageSize;

//        this.cities = new MatTableDataSource<City>(result.data);
//      }, error => console.error(error));
//  }

//}
