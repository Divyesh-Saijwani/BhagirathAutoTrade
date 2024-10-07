import { Roles } from "./common.model";

export class Menus{
    id!: number;
    menuTitle!:string;
    menuLink!:string;
    roles:Roles[]=[];
    active!:boolean;
    subMenu!: Menus[];
    iconClass!:string;
}