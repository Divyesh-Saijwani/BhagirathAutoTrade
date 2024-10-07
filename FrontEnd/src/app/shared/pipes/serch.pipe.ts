import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'serch'
})
export class SerchPipe implements PipeTransform {

  transform(items: any[], searchText: string): any[] {
    if (!items) {
      return [];
    }
    if (!searchText) {
      return items;
    }
    searchText = searchText.toLocaleLowerCase();

    return items.filter(it => {
      if(it.fullName){
        return it.fullName.toLocaleLowerCase().includes(searchText);
      }else if(it.description){
        return it.description.toLocaleLowerCase().includes(searchText);
      }
    });
  }

}
