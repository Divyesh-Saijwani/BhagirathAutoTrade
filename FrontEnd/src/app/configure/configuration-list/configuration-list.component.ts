import { Component, HostBinding } from '@angular/core';
import { DashboardService } from '../../shared/services/dashboard.service';

@Component({
  selector: 'app-configuration-list',
  templateUrl: './configuration-list.component.html',
  styleUrl: './configuration-list.component.scss'
})
export class ConfigurationListComponent {
  @HostBinding('class') public classes: string =
    'd-flex flex-column h-100 overflow-hidden';
  searchText: string = '';
  public broker: any;
  public configList: any = [];
  constructor(private dashboardService: DashboardService) { } 

  ngOnInit(): void {
    this.dashboardService.GetUserBrokerConfigurationStatus('7D4A58AA-1EF6-42B1-98F6-A45146C9CB6D').subscribe((data:any)=>{
      this.broker=data[0];
      data.forEach((item: any) => {
        item.brokerConfigurations.forEach((el:any)=>{
          var config={
            id:el.brokerConfigurationId,
            brokerName : item.name,
            configName:el.configKey,
            configUpdatedValue:""
          }
          this.configList.push(config);
        })
        
      });
    });
  }

  updateConfiguration(configId:string, updatedValue:string){

  }
}
