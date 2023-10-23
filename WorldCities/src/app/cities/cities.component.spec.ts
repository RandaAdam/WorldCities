import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AngularMaterialModule } from '../angular-material.module';
import { RouterTestingModule } from '@angular/router/testing';
import { CitiesComponent } from './cities.component';
import { CityService } from './city.service';
import { ApiResult } from '../base.service';
import { City } from './city';
import { of } from 'rxjs';

describe('CitiesComponent', () => {
  let component: CitiesComponent;
  let fixture: ComponentFixture<CitiesComponent>;

  beforeEach(async () => {
    //ctrate mock
    let cityService = jasmine
      .createSpyObj<CityService>('CityService', ['getData']);

    //configure the 'getData' spy method
    cityService.getData.and.returnValue(
      //return an observable with some test data
      of<ApiResult<City>>(<ApiResult<City>>{
        data: [
          <City>{
            name: `TestCity1`,
            id: 1,
            lat: 1,
            lon: 1,
            countryId: 1,
            CountryName: 'TestCounrty1'
          },
          <City>{
            name: `TestCity2`,
            id: 2,
            lat: 1,
            lon: 1,
            countryId: 1,
            CountryName: 'TestCounrty1'
          },
          <City>{
            name: `TestCity3`,
            id: 3,
            lat: 1,
            lon: 1,
            countryId: 1,
            CountryName: 'TestCounrty1'
          }
        ],
        totalCount: 3,
        pageIndex: 0,
        pageSize: 10
      })
    );


    await TestBed.configureTestingModule({
      declarations: [CitiesComponent],
      imports: [
        BrowserAnimationsModule,
        AngularMaterialModule,
        RouterTestingModule
      ],
      providers: [
        {
          provide: CityService,
          useValue: cityService
        }
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CitiesComponent);
    component = fixture.componentInstance;
    // configure fixture/component/children/etc.
    component.paginator = jasmine.createSpyObj(
      "MatPaginator", ["length", "pageIndex", "pageSize"]
    );
    fixture.detectChanges();
  })

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  // implement some other tests
  it('should display a Cities title', () => {
    let title = fixture.nativeElement
      .querySelector('h1');
    expect(title.textContent).toEqual('Cities');
  });

  //second test
  it('should contain a table with a list of one or more cities', () => {
    let table = fixture.nativeElement
      .querySelector('table.mat-table');
    let tableRows = table
      .querySelectorAll('tr.mat-row');
    expect(tableRows.length).toBeGreaterThan(0);
  });
});
