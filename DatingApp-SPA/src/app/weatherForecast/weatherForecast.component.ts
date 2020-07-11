import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-weatherForecast',
  templateUrl: './weatherForecast.component.html',
  styleUrls: ['./weatherForecast.component.css']
})
export class WeatherForecastComponent implements OnInit {

  constructor(private http: HttpClient) { }
  values: any;
  ngOnInit() {
    this.getWeatherForecasts();
  }

  getWeatherForecasts() {
    this.http.get('http://localhost:5000/WeatherForecast/').subscribe(response => {
      this.values = response;
    }, error => {
      console.log(error);
    })
  }

}
