import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { EquityData } from './models/equity-data';
import { FormControl } from '@angular/forms';
import { map, startWith } from 'rxjs';
import { StocksService } from './services/stocks.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {

  title = 'Bhagirath Auto Trade App';

  @ViewChild('downloadLink')
    downloadLink!: ElementRef;

  public equitydata: EquityData = {
      Id: 0,
      WorkingDate: null,
      Exchange: '',
      Type: '',
      Instrument: '',
      OptionType: 'XX',
      Symbole: '',
      ExpiryDate: null,
      StrikePrice: '',
      Open: '',
      High: '',
      Low: '',
      Cmp: '',
      Close: '',
      FileType: 0,
      UploadDateTime: '',
      FileNo: 0,
      Average: '',
      TotalTradeValue: '',
      MST: '',
      MSTS: '',
      SS: '',
      RS: '',
      SST: '',
      RST: '',
      HS: '',
      HR: ''
  };
  public datePipe: DatePipe = new DatePipe('en-US');
  public optidxstxoption: boolean = false;
  public derivativeoption: boolean = false;
  //public optidxstxoption: boolean = true;
  //public optidxstxoption: boolean = true;
  public strikePrices: string[] = [];
  public currentDate: string = '';

  searchControl = new FormControl();
  options: string[] = [];
  filteredOptions: string[] = [];
  expiryDates: string[] = [];

  constructor(private stocksService: StocksService) {
    this.setCurrentDate();
  }

  ngOnInit() {
    //$('#autoCompleteTextbox').typeahead({
    //  source: ['Option 1', 'Option 2', 'Option 3', 'Option 4'] // Data source for auto-completion
    //});

    this.searchControl.disable();
    this.searchControl.valueChanges
      .pipe(
        startWith(''),
        map(value => this._filter(value))
      )
      .subscribe(options => this.filteredOptions = options);
  }

  onReset() {
    this.equitydata = {
      Id: 0,
      WorkingDate: null,
      Exchange: '',
      Type: '',
      Instrument: '',
      OptionType: 'XX',
      Symbole: '',
      ExpiryDate: null,
      StrikePrice: '',
      Open: '',
      High: '',
      Low: '',
      Cmp: '',
      Close: '',
      FileType: 0,
      UploadDateTime: '',
      FileNo: 0,
      Average: '',
      TotalTradeValue: '',
      MST: '',
      MSTS: '',
      SS: '',
      RS: '',
      SST: '',
      RST: '',
      HS: '',
      HR: ''
    };
    this.expiryDates = [];
    this.strikePrices = [];
  }

  onDateOrExpiryChange() {
    this.resetStockData();
    if (this.equitydata.Symbole && this.equitydata.ExpiryDate && this.equitydata.WorkingDate) {
      this.stocksService.getCloseData(this.equitydata).subscribe(result => {
        this.equitydata.Close = result.toString();
        this.searchStockData();
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

  onOptionTypeChange() {
    if (['CE', 'PE'].includes(this.equitydata.Type)) {

    }
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
                this.stocksService.autoCompleteCompanyForEquity(this.equitydata.Exchange, this.equitydata.Type, this.equitydata.Instrument, this.equitydata.OptionType)
                  .subscribe(options => {
                    this.options = options;
                  });
                return;
              }
            } else {
              this.stocksService.autoCompleteCompanyForEquity(this.equitydata.Exchange, this.equitydata.Type, this.equitydata.Instrument, this.equitydata.OptionType)
                .subscribe(options => {
                  this.options = options;
                });
              return;
            }
          }
        } else {
          // Make API call to fetch options
          this.stocksService.autoCompleteCompanyForEquity(this.equitydata.Exchange, this.equitydata.Type, this.equitydata.Instrument, this.equitydata.OptionType)
            .subscribe(options => {
              this.options = options;
            });
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
    this.equitydata.Symbole = '';
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
    this.equitydata.Symbole = '';
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

  searchStockData() {
    //if (this.equitydata.Close == '' || this.equitydata.Close == null || this.equitydata.Close == undefined) {
    //  this.onDateOrExpiryChange();
    //}

    this.stocksService.getCalculateDataForEQ(this.equitydata).subscribe(result => {
      this.equitydata.Open = result.open;
      this.equitydata.Average = result.average;
      this.equitydata.High = result.high;
      this.equitydata.Low = result.low;
      this.equitydata.Cmp = result.cmp;
    })
  }

  resetStockData() {
    this.equitydata.Open = "";
    this.equitydata.Average = "";
    this.equitydata.High = "";
    this.equitydata.Low = "";
    this.equitydata.Cmp = "";
    this.equitydata.Close = "";
    this.equitydata.SS = "";
    this.equitydata.SST = "";
    this.equitydata.RS = "";
    this.equitydata.RST = "";
    this.equitydata.HS = "";
    this.equitydata.HR = "";
  }

  downloadExcel() {
    this.stocksService.downloadExcel(this.equitydata).subscribe((data: Blob) => {
      // Create a Blob URL for the received Blob data
      const blobUrl = window.URL.createObjectURL(data);

      // Set the href and download attributes of the <a> tag
      this.downloadLink.nativeElement.href = blobUrl;
      this.downloadLink.nativeElement.download = 'UpdatedFile.xlsx';

      // Programmatically trigger a click event on the <a> tag to start the download
      this.downloadLink.nativeElement.click();

      // Revoke the Blob URL to release memory
      window.URL.revokeObjectURL(blobUrl);
    }, error => {
      console.error('Error downloading Excel file:', error);
    });
  }

}
