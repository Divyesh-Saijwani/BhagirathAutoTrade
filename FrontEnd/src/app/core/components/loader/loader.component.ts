/**
 * @author Gaurang Valia
 * @description Provides the loader throughout the system.
 */

import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  Input,
  OnDestroy,
  OnInit,
  input,
} from '@angular/core';
import { LoaderService } from './loader.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-loader',
  templateUrl: './loader.component.html',
  styleUrls: ['loader.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoaderComponent implements OnInit, OnDestroy {
  /**
   * provides the visibility of a loader.
   */
  @Input() public loaderVisibility: boolean = false;

  /**
   * provides the loader Image Path.
   */
  public imagePath: string;

  /**
   * subscription for loader status.
   */
  public loaderSubscription!: Subscription;
  /**
   * @author Gaurang Valia
   * @constructor the Constructor.
   */
  constructor(
    private loaderService: LoaderService,
    private changeDetection: ChangeDetectorRef
  ) {
    this.imagePath = '../assets/images/loader.gif';
  }

  /**
   * @author Gaurang Valia.
   * @description initializes the subscription.
   */
  public ngOnInit(): void {
    this.loaderSubscription = this.loaderService
      .getLoaderStatus()
      .subscribe((status: boolean) => {
        this.changeDetection.markForCheck();
        this.loaderVisibility = status;
      });
  }

  /**
   * @author Gaurang Valia
   * @description clears the memories for this component.
   */
  public ngOnDestroy(): void {
    this.loaderSubscription.unsubscribe();
  }
}
