import { Component, ElementRef, HostBinding, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DashboardService } from '../shared/services/dashboard.service';
import { Calculation } from './models/calculation';
import { EquityData } from './models/equity-data';
import { StocksService } from '../shared/services/stocks.service';
import { DatePipe } from '@angular/common';
import { FormControl } from '@angular/forms';
import { map, startWith } from 'rxjs';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { AuthService } from '../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  @HostBinding('class') public classes: string = 'd-flex flex-column h-100 overflow-hidden';

  @ViewChild('downloadLink')
    downloadLink!: ElementRef;

  public broker: any;
  public calculationList!: Calculation[];
  public optidxstxoption: boolean = false;
  public derivativeoption: boolean = false;
  //public optidxstxoption: boolean = true;
  //public optidxstxoption: boolean = true;
  public expiry: string[] = [];
  public currentDate: string = '';

  isAdmin:boolean=false;

  searchControl = new FormControl();
  options: string[] = ["NIFTY", "BANKNIFTY", "FINNIFTY"];
  filteredOptions: string[] = [];
  expiryDates: string[] = [];
  public datePipe: DatePipe = new DatePipe('en-US');

  public equitydata: EquityData = {
    Id: 0,
    WorkingDate: null,
    Exchange: '',
    Type: '',
    Instrument: '',
    OptionType: 'XX',
    Symbol: '',
    ExpiryDate: null,
    StrikePrice: '',
    SS: '',
    RS: '',
    SST: '',
    RST: '',
    HS: '',
    HR: ''
};

  constructor(private route: ActivatedRoute,
    private routing: Router,private dashboardService: DashboardService,private stocksService: StocksService,public authService:AuthService) {
      this.setCurrentDate();
      let userDetails=this.authService.getLoggedinUserDetails();
      this.isAdmin=userDetails.roles.includes("Admin");
  }
  


ngOnInit() {
  //$('#autoCompleteTextbox').typeahead({
  //  source: ['Option 1', 'Option 2', 'Option 3', 'Option 4'] // Data source for auto-completion
  //});

  this.equitydata.WorkingDate=this.datePipe.transform(new Date(), 'MM/dd/yyyy');

  this.searchControl.disable();
  this.searchControl.valueChanges
    .pipe(
      startWith(''),
      map(value => this._filter(value))
    )
    .subscribe(options => this.filteredOptions = options);

    this.calculationList=[];
    this.dashboardService.GetUserBrokerConfigurationStatus('7D4A58AA-1EF6-42B1-98F6-A45146C9CB6D').subscribe((data:any)=>{
      this.broker=data[0];
    });

    this.dashboardService.GetTodayCalculations().subscribe((data: Calculation[]) => {
      this.calculationList = data;
    },
    (error) => {
      console.error('Error fetching calculations', error);
    }); 
}

onReset() {
  this.equitydata = {
    Id: 0,
    WorkingDate: this.datePipe.transform(new Date(), 'MM/dd/yyyy'),
    Exchange: '',
    Type: '',
    Instrument: '',
    OptionType: 'XX',
    Symbol: '',
    ExpiryDate: null,
    StrikePrice: '',
    SS: '',
    RS: '',
    SST: '',
    RST: '',
    HS: '',
    HR: ''
  };
}

onDateOrExpiryChange() {
  this.resetStockData();
  if (this.equitydata.Symbol && this.equitydata.ExpiryDate && this.equitydata.WorkingDate) {
    this.stocksService.getCloseData(this.equitydata).subscribe(result => {
    });
  }
}

onExchangeChange() {
  this.equitydata.Type = '';
  this.equitydata.Instrument = '';
  this.equitydata.OptionType = '';
  this.options = [];
  this.enableSymbolSearch();
}

enableSymbolSearch() {
  if (this.equitydata.Exchange && this.equitydata.Type) {
    if (['DERIVATIVE'].includes(this.equitydata.Type)) {

      if (this.equitydata.Instrument) {
        if (['OPTIDX', 'OPTSTK'].includes(this.equitydata.Instrument)) {
          if (this.equitydata.OptionType) {
            // Make API call to fetch options
            this.searchControl.enable();
            return;
          }
          this.searchControl.disable();
        } else {
          this.searchControl.enable();
          return;
        }
      } else {
        this.searchControl.disable();
      }
    } else {
      this.searchControl.enable();
    }
  } else {
    this.searchControl.disable();
  }
}

fetchOptions(): void {
  if (!this.options || this.options != null || this.options == undefined) {
    // Check if all parameters are available
    if (this.equitydata.Exchange && this.equitydata.Type) {
      if (['DERIVATIVE'].includes(this.equitydata.Type)) {
        if (this.equitydata.Instrument) {
          if (['OPTIDX', 'OPTSTK'].includes(this.equitydata.Instrument)) {
            if (this.equitydata.OptionType) {
              // Make API call to fetch options
              this.options =this.stocksService.autoCompleteCompanyForEquity(this.equitydata.Exchange, this.equitydata.Type, this.equitydata.Instrument, this.equitydata.OptionType);
              return;
            }
          } else {
            // this.stocksService.autoCompleteCompanyForEquity(this.equitydata.Exchange, this.equitydata.Type, this.equitydata.Instrument, this.equitydata.OptionType)
            //   .subscribe(options => {
            //     this.options = options;
            //   });
            this.options =this.stocksService.autoCompleteCompanyForEquity(this.equitydata.Exchange, this.equitydata.Type, this.equitydata.Instrument, this.equitydata.OptionType);

            return;
          }
        }
      } else {
        // Make API call to fetch options
        // this.stocksService.autoCompleteCompanyForEquity(this.equitydata.Exchange, this.equitydata.Type, this.equitydata.Instrument, this.equitydata.OptionType)
        //   .subscribe(options => {
        //     this.options = options;
        //   });
        this.options =this.stocksService.autoCompleteCompanyForEquity(this.equitydata.Exchange, this.equitydata.Type, this.equitydata.Instrument, this.equitydata.OptionType);
      }

    } else {
      // If any parameter is missing, clear options and disable autocomplete control
      this.options = [];
    }
  }
}

private _filter(value: string): string[] {
  const filterValue = value.toLowerCase();
  return this.options.filter(option => option.toLowerCase().includes(filterValue));
}

displayFn(value: string): string {
  return value ? value : '';
}

setCurrentDate(): void {
  const today = new Date();
  const year = today.getFullYear();
  const month = (today.getMonth() + 1).toString().padStart(2, '0'); // Adding leading zero if needed
  const day = today.getDate().toString().padStart(2, '0'); // Adding leading zero if needed
  this.currentDate = `${year}-${month}-${day}`;
}

onTypeOptionUpdate(): void {
  // Check if the selected instrument is FUTIDX, FUTIVX, or FUTSTK
  if (['DERIVATIVE'].includes(this.equitydata.Type)) {
    this.derivativeoption = true; // Set optidxstxoption to false
  } else {
    this.derivativeoption = false; // Set optidxstxoption to true for other options
  }
  this.equitydata.Instrument = '';
  this.equitydata.OptionType = '';
  this.options = [];
  this.equitydata.Symbol = '';
  this.equitydata.WorkingDate = '';
  this.enableSymbolSearch();
  this.resetStockData();
}

onInstrumentOptionUpdate(): void {
  // Check if the selected instrument is FUTIDX, FUTIVX, or FUTSTK
  if (['OPTIDX', 'OPTSTK'].includes(this.equitydata.Instrument)) {
    this.optidxstxoption = true; // Set optidxstxoption to false
  } else {
    this.optidxstxoption = false; // Set optidxstxoption to true for other options
  }
  this.equitydata.OptionType = '';
  this.options = [];
  this.equitydata.Symbol = '';
  this.equitydata.WorkingDate = '';
  this.equitydata.ExpiryDate = '';
  this.enableSymbolSearch();
  this.resetStockData();
}

onOptionSelected(event: MatAutocompleteSelectedEvent): void {
  const selectedSymbol = event.option.value;
  // Call your getexpiredate() method here with the selected symbol
  this.stocksService.getexpiredate(selectedSymbol, this.equitydata.OptionType).subscribe((result: string[]) => {
    this.expiryDates = [];
    result.forEach((x, i) => {
      var selectedexpiry = this.datePipe.transform(x, 'MM/dd/yyyy');
      var expireDate = null;
      if (selectedexpiry) {
        this.expiryDates.push(selectedexpiry)
        expireDate = new Date(selectedexpiry);
      }

      var todayNewDate = new Date();

      if (expireDate && expireDate >= todayNewDate) {
        this.equitydata.ExpiryDate = selectedexpiry;
      }
    });
    this.onDateOrExpiryChange();
  });
}


resetStockData() {
  this.equitydata.SS = "";
  this.equitydata.SST = "";
  this.equitydata.RS = "";
  this.equitydata.RST = "";
  this.equitydata.HS = "";
  this.equitydata.HR = "";
}

ngCalculateEquity() {
  this.equitydata.WorkingDate
  this.equitydata.WorkingDate=this.datePipe.transform(new Date(), 'MM/dd/yyyy');
  this.equitydata.OptionType="XX";
  this.equitydata.StrikePrice='0';

  this.stocksService.downloadExcel(this.equitydata).subscribe((data: Blob) => {
    // // Create a Blob URL for the received Blob data
    // const blobUrl = window.URL.createObjectURL(data);

    // // Set the href and download attributes of the <a> tag
    // this.downloadLink.nativeElement.href = blobUrl;
    // this.downloadLink.nativeElement.download = 'UpdatedFile.xlsx';

    // // Programmatically trigger a click event on the <a> tag to start the download
    // this.downloadLink.nativeElement.click();

    // // Revoke the Blob URL to release memory
    // window.URL.revokeObjectURL(blobUrl);
    this.dashboardService.GetTodayCalculations().subscribe((data: Calculation[]) => {
      this.calculationList = data;
      this.onReset();
    },
    (error) => {
      console.error('Error fetching calculations', error);
    }); 
  }, error => {
    console.error('Error downloading Excel file:', error);
  });
}

}
