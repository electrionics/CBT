import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public cognitiveErrors: CognitiveError[] = [];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<CognitiveError[]>('https://localhost:7200/' + 'AutomaticThougths/GetAllCognitiveErrors').subscribe((result: CognitiveError[]) => {
      this.cognitiveErrors = result;
    }, (error: any) => console.error(error));
  }
}

interface CognitiveError {
  key: number;
  value: string;
}

interface ThreeColumnsItem {
  id: number;
  thought: string;
  errors: number[];
  rationalAnswer: string;
}
