export class Broker {
    public brokerId?: string;
    public name!: string;
    public description!: string;
    public apiDocumentationUrl!: string;
    public authenticationUrl!: string;
    public isActive!: boolean;
    public brokerConfigurations: BrokerConfiguration[];
  
    constructor() {
      this.brokerConfigurations = [];
    }
  }
  
  export class BrokerConfiguration {
    public brokerConfigurationId?: string;
    public brokerId?: string;
    public configKey!: string;
    public needsToUpdateDaily: boolean;
  
    constructor() {
      this.needsToUpdateDaily = false;
    }
  }