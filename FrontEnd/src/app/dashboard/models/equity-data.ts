export interface EquityData {
  Id: number;
  Exchange: string;
  Type: string;
  Instrument: string;
  OptionType: string;
  Symbol: string;
  WorkingDate: string | null;
  ExpiryDate: string | null;
  StrikePrice: string;
  SS: string;
  RS: string;
  SST: string;
  RST: string;
  HS: string;
  HR: string;
}
