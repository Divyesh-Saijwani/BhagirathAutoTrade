export interface UserAlgoData{
    id:string;
    workingDate: Date | null;
    symbol: string;
    exchange: string;
    type: string;
    instrument: string;
    optionType: string;
    expiryDate: string | null;
    callStrikePrice: string;
    putStrikePrice: string;
    direction:string;
    noOfLots:number;
    multiple:number;
    status:number;
}