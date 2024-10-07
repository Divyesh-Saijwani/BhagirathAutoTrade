import { Component, HostBinding } from '@angular/core';
import { UsersService } from '../../shared/services/users.service';
import { InventoryService } from '../inventory.service';
import * as XLSX from 'xlsx';
import { ActivatedRoute, Router } from '@angular/router';
@Component({
  selector: 'app-inventory-list',
  templateUrl: './inventory-list.component.html',
  styleUrl: './inventory-list.component.scss',
})
export class InventoryListComponent {
  @HostBinding('class') public classes: string = 'd-flex flex-column h-100 overflow-hidden';
  public inventoryList: any[] = [];
  searchText: string = '';
  jsonDatails: any = null;
  constructor(
    private route: ActivatedRoute,
    private routing: Router,
    private inventoryService: InventoryService
  ) {
    this.getInventory();
  }
  openFile() {
    document.getElementById('in1')?.click();
  }
  getInventory() {
    this.inventoryService.getAllInventory().subscribe((el) => {
      this.inventoryList = el.body;
    });
  }
  onFileChange(value: any) {
    let workBook: XLSX.WorkBook | null = null;
    let jsonData = null;
    const reader = new FileReader();
    const file = value.target.files[0];
    reader.onload = (event) => {
      const data = reader.result;
      let workBook = XLSX.read(data, { type: 'binary' });
      jsonData = workBook.SheetNames.reduce((initial: any, name:any) => {
        const sheet = workBook.Sheets[name];
        initial[name] = XLSX.utils.sheet_to_json(sheet);
        return initial;
      }, {});
      const dataString = JSON.stringify(jsonData);
      if(jsonData != null){
        this.jsonDatails = jsonData.All_Material_List
        this.importDetails()
      }
      // document.getElementById('output').innerHTML = dataString.slice(0, 300).concat("...");
      // this.setDownload(dataString);
    };
    reader.readAsBinaryString(file);
  }

  importDetails() {
    this.inventoryService.importFile(this.jsonDatails).subscribe((el) => {
      this.getInventory();
    });
  }

  exportToCsv() {
    const data = this.inventoryList;
    const columns = this.getColumns(data);
    const csvData = this.convertToCsv(data, columns);
    this.downloadFile(csvData, 'data.csv', 'text/csv');
  }

  getColumns(data: any[]): string[] {
    const columns:any = [];
    data.forEach(row => {
      Object.keys(row).forEach(col => {
        if (!columns.includes(col)) {
          columns.push(col);
        }
      });
    });
    return columns;
  }

  convertToCsv(data: any[], columns: string[]): string {
    let csv = '';
    csv += columns.join(',') + '\n';
    data.forEach(row => {
      const values:any = [];
      columns.forEach(col => {
        values.push(row[col] || '');
      });
      csv += values.join(',') + '\n';
    });
    return csv;
  }

  downloadFile(data: string, filename: string, type: string) {
    const blob = new Blob([data], { type: type });
    const url = URL.createObjectURL(blob);
    
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    link.click();
    
    // Clean up after download
    URL.revokeObjectURL(url);
  }
}
