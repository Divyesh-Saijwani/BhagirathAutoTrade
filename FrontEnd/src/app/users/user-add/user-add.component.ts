import { Component, EventEmitter, Output } from '@angular/core';
import { OverlayInputConfig } from '../../shared/components/overlay/overlay.model';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { StocksService } from '../../shared/services/stocks.service';
import { UserAlgoData } from '../../shared/models/UserAlgoData';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-user-add',
  templateUrl: './user-add.component.html',
  styleUrl: './user-add.component.scss',
})
export class UserAddComponent {
  /** provides the configuration for All the inputs. */
  public inputConfig!: OverlayInputConfig;
  
  public strikePrices: string[] = [];
  public currentDate: string = '';
  expiryDates: string[] = [];
  public optidxstxoption: boolean = false;
  public derivativeoption: boolean = false;
  public datePipe: DatePipe = new DatePipe('en-US');
  public algoFormGroup: UserAlgoData;

  searchControl = new FormControl();
  options: string[] = ["NIFTY"];
  filteredOptions: string[] = ["NIFTY"];

  constructor(
    private routing: Router,
    private stockService: StocksService
  ) {
    this.algoFormGroup = {
      id:'',
      workingDate: new Date(),
      symbol: '',
      exchange: '',
      type: '',
      instrument: '',
      optionType: '',
      expiryDate: null,
      callStrikePrice: '',
      putStrikePrice: '',
      direction:'BUY',
      noOfLots:1,
      multiple:1,
      status:0,
  }
    this.inputConfig = {
      title: 'Add APIs Details',
      isSubmitEnable: true,
      isFormTouched: false,
    };    
  }

  onExchangeChange() {
    this.algoFormGroup.type = '';
    this.algoFormGroup.instrument = '';
    this.algoFormGroup.optionType = '';
    this.algoFormGroup.callStrikePrice = '';
    this.algoFormGroup.putStrikePrice = '';
    this.algoFormGroup.expiryDate = null; 
  }

  onTypeOptionUpdate(): void {
    // Check if the selected instrument is FUTIDX, FUTIVX, or FUTSTK
    if (['DERIVATIVE'].includes(this.algoFormGroup.type)) {
      this.derivativeoption = true; // Set optidxstxoption to false
    } else {
      this.derivativeoption = false; // Set optidxstxoption to true for other options
    }
    this.algoFormGroup.instrument = '';
    this.algoFormGroup.optionType = '';
    this.algoFormGroup.symbol = '';
  }

  onInstrumentOptionUpdate(): void {
    // Check if the selected instrument is FUTIDX, FUTIVX, or FUTSTK
    if (['OPTIDX', 'OPTSTK'].includes(this.algoFormGroup.instrument)) {
      this.optidxstxoption = true; // Set optidxstxoption to false
    } else {
      this.optidxstxoption = false; // Set optidxstxoption to true for other options
    }
    this.algoFormGroup.optionType = '';
    this.algoFormGroup.symbol = '';
    this.algoFormGroup.expiryDate = null;
    this.fetchOptions();
  }

  onOptionSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedSymbol = event.option.value;
    // Call your getexpiredate() method here with the selected symbol
    this.stockService.getexpiredate(selectedSymbol, this.algoFormGroup.optionType).subscribe((result: string[]) => {
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
          this.algoFormGroup.expiryDate = selectedexpiry;
        }
      });
      this.stockService.getStrikePrice(selectedSymbol).subscribe((res:string[])=>{
        this.strikePrices =res;
      });
    });
  }

  fetchOptions(): void {
    if (!this.options || this.options != null || this.options == undefined) {
      // Check if all parameters are available
      if (this.algoFormGroup.exchange && this.algoFormGroup.type) {
        if (['DERIVATIVE'].includes(this.algoFormGroup.type)) {
          if (this.algoFormGroup.instrument) {
            if (['OPTIDX', 'OPTSTK'].includes(this.algoFormGroup.instrument)) {
              if (this.algoFormGroup.optionType) {
                // Make API call to fetch options
                this.options =this.stockService.autoCompleteCompanyForEquity(this.algoFormGroup.exchange, this.algoFormGroup.type, this.algoFormGroup.instrument, this.algoFormGroup.optionType);
                return;
              }
            } else {
              this.options =this.stockService.autoCompleteCompanyForEquity(this.algoFormGroup.exchange, this.algoFormGroup.type, this.algoFormGroup.instrument, this.algoFormGroup.optionType);

              return;
            }
          }
        } else {
          // Make API call to fetch options
          this.options =this.stockService.autoCompleteCompanyForEquity(this.algoFormGroup.exchange, this.algoFormGroup.type, this.algoFormGroup.instrument, this.algoFormGroup.optionType);
        }

      } else {
        // If any parameter is missing, clear options and disable autocomplete control
        this.options = [];
      }
    }
  }

  displayFn(value: string): string {
    return value ? value : '';
  }

  fetchStrikePrice() {
    if (this.algoFormGroup.symbol && this.algoFormGroup.type && this.algoFormGroup.instrument) {
      if (['DERIVATIVE'].includes(this.algoFormGroup.type)) {
        if (this.algoFormGroup.instrument) {
          if (['OPTIDX', 'OPTSTK'].includes(this.algoFormGroup.instrument)) {
              // Make API call to fetch options
              this.stockService.getStrikePrice(this.algoFormGroup.symbol).subscribe((res:string[])=>{
                this.strikePrices =res;
              });
              return;
          }
        }
      }
    }
  }

  onCancel() {
    this.routing.navigate(['../apis']);
  }

  onSubmit() {
    this.stockService.addUserAlgoData(this.algoFormGroup).subscribe((res) => {
      this.stockService.refreshEvent.emit(true);
      this.routing.navigate(['../apis']);
    });
  }
}
