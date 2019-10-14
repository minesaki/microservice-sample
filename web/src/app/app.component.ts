import { Component, ChangeDetectionStrategy } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import * as uuid from 'uuid';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.styl'],
})
export class AppComponent {

  public omikuji_result: string = '';

  constructor(private _http: HttpClient) {
  }

  public async onInputClick(name: string): Promise<void> {
    try{
      const response: any = await this._http.post<string>('/api/omikuji', {IdempotencyKey: uuid.v4(), UserName: name}).toPromise();
      if(response.error){
        this.omikuji_result = response.error;
      }else{
        this.omikuji_result = response.omikujiResult;
      }
    } catch {
      this.omikuji_result = '## 現在サービス停止中です ##';
    }
  }

  public async onKeyDown(keyCode: number, name: string){
    if(keyCode === 13){
      await this.onInputClick(name);
    }
  }

}
