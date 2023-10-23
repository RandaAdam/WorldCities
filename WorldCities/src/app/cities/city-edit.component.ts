//import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment.prod';
import { BaseFormComponent } from '../baseform.component';
import { Country } from '../countries/country';
import { City } from './city';
import { CityService } from './city.service';

@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrls: ['./city-edit.component.scss']
})
export class CityEditComponent extends BaseFormComponent implements OnInit {

  //the view title
  title?: string;
  //the city to be edit or create
  city?: City;

  //city id, is null if adding new
  id?: number;

  //the countries observable for the select (using async pipe)
  countries?: Observable<Country[]>;

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private cityService: CityService) {
    super();
  }

  ngOnInit(): void {
    this.form = new FormGroup({
      name: new FormControl('', Validators.required),
      lat: new FormControl('', [
        Validators.required,
        Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/)
      ]),
      lon: new FormControl('', [
        Validators.required,
        Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/)
      ]),
      countryId: new FormControl('', Validators.required)
    }, null, this.isDupeCity());
    this.loadData();
  }

  loadData(): void {
    this.loadCountries();
    //the id from 'id' parameter
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;

    //Edit mode
    if (this.id) {
      //fetch city from server
      var url = environment.baseUrl + 'api/cities/' + this.id;
      this.cityService.get(this.id)
        .subscribe(result => {
          this.city = result;
          this.title = "Edit - " + this.city.name;

          //update the form with the city value
          this.form.patchValue(this.city);
        }, error => console.log(error));
    }//add-new mode
    else {
      this.title = "Create new City";
    }
  }

  loadCountries() {

    this.countries = this.cityService.getCountries(
      0,
      9999,
      "name",
      "asc",
      null,
      null
    ).pipe(map(result => result.data));

    //  subscribe(result => {
    //  this.countries = result.data;
    //}, error => console.log(error));
  }


  onSubmit() {
    var city = (this.id) ? this.city : <City>{};
    if (city) {
      city.name = this.form.controls['name'].value;
      city.lat = +this.form.controls['lat'].value;
      city.lon = +this.form.controls['lon'].value;
      city.countryId = +this.form.controls['countryId'].value;

      //edit mode
      if (this.id) {
        //var url = environment.baseUrl + 'api/cities/' + city.id;
        //this.http
        //  .put<City>(url, city)
        //  .subscribe(result => {
        //    console.log("City " + city!.id + " has been updated");

        //    //back to cities view
        //    this.router.navigate(['/cities']);
        //  }, error => console.log(error));
        this.cityService.put(city)
          .subscribe(result => {
            console.log("City " + city!.id + " has been updated.");
            //back to cities view
            this.router.navigate(['/cities']);
          }, error => console.log(error));
      } else {
        //Add-new mode
        //var url = environment.baseUrl + 'api/cities';
        //this.http
        //  .post<City>(url, city)
        //  .subscribe(result => {
        //    console.log("City " + result.id + " has been created");
        //    //back to cities view
        //    this.router.navigate(['/cities']);

        //  }, error => console.log(error));

        this.cityService.post(city)
          .subscribe(result => {
            console.log("City " + result.id + " has been created");
            //back to cities view
            this.router.navigate(['/cities']);
          }, error => console.log(error));
      }
    }
  }

  isDupeCity(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{ [key: string]: any } |
      null> => {
      var city = <City>{};
      city.id = (this.id) ? this.id : 0;
      city.name = this.form.controls['name'].value;
      city.lat = this.form.controls['lat'].value;
      city.lon = this.form.controls['lon'].value;
      city.countryId = this.form.controls['countryId'].value;

      var url = environment.baseUrl + 'api/cities/IsDupeCity';
      return this.cityService.isDupeCity(city)
        .pipe(map(result => {
          return (result ? { isDupeCity: true } : null);
        }));
    }
  }
}

