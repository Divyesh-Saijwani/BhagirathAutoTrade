export interface Calculation {
    algoCalculationId: string;
    symbol: string;
    exchange: string;
    type?: string;
    instrument?: string;
    optionType?: string;
    strikePrice?: number;
    trend?:string;
    expiry?: string;
    lowPoint: number;
    averagePoint: number;
    maxPoint: number;
    direction: string;
    entryTime: string;
    stopLoss: number;
    target: number;
    calculationDateTime: string;
    isActive: boolean;
}

