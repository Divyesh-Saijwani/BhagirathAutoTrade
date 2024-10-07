export class RequestLoginDetails {
  public email!: string;
  public password!: string;
}

export const userGroupList = [
  {
    label: 'Associate',
    value: 2,
  },
  {
    label: 'Trader',
    value: 3,
  },
];
export const AdminGroupList = [
  {
    label: 'Admin',
    value: 1,
  },
];

export const roleList = [
  {
    userGroupType: 1,
    roles: [
      {
        label: 'Self',
        value: 1,
      },
      {
        label: 'Staff',
        value: 2,
      },
      {
        label: 'Trader',
        value: 3,
      },
    ],
  },
  {
    userGroupType: 2,
    roles: [
      {
        label: 'Self',
        value: 1,
      },
      {
        label: 'Contact Person',
        value: 2,
      },
    ],
  },
  {
    userGroupType: 3,
    roles: [
      {
        label: 'Renovator',
        value: 1,
      },
    ],
  },
  {
    userGroupType: 4,
    roles: [
      {
        label: 'Home Owner',
        value: 1,
      },
    ],
  },
];

export class Users {
  public id?: string;
  public email!: string;
  public password!: string;
  public fullName!: string;
  public phoneNumber!: string;
  public role!: string;
  public isActive?: string;
}

export enum Roles {
  Admin = "Admin",
  Associate="Associate",
  Trader="Trader",
}

export const RoleDetailsList = [
  {
    role:Roles.Admin,
    actions:[
      {
        module_name: 'dashboard',
        create_action: true,
        read_action: true,
        update_action: true,
        delete_action: true,
      },
      {
        module_name: 'users',
        create_action: true,
        read_action: true,
        update_action: true,
        delete_action: true,
      },
      {
        module_name: 'quotations',
        create_action: true,
        read_action: true,
        update_action: true,
        delete_action: true,
      },
      {
        module_name: 'inventory',
        create_action: true,
        read_action: true,
        update_action: true,
        delete_action: true,
      },
      {
        module_name: 'customer',
        create_action: true,
        read_action: true,
        update_action: true,
        delete_action: true,
      },
      {
        module_name: 'broker',
        create_action: true,
        read_action: true,
        update_action: true,
        delete_action: true,
      },
    ]
  }
  
];
