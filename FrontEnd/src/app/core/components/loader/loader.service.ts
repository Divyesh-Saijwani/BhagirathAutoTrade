/**
 * @author Gaurang Valia
 * @description Provides the loader for time taking task
 * (user can calls the display loader method by passing boolean value which describes that loader should be display or not.)
 */

import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
    /** to provide an object of this service in the root injector. */
    providedIn: 'root',
})
export class LoaderService {

    constructor() {
        this.loaderStatus = new BehaviorSubject(false);
    }

    /**
     * provides the loader status for loader to be displayed
     */
    public loaderStatus: BehaviorSubject<boolean>;

    /**
     * @author Gaurang Valia
     * @description determines that loader to be displayed or not.
     * @param status the loader Status
     */
    public displayLoader(status: boolean): void {
        this.loaderStatus.next(status);
    }

    /**
     * @author Gaurang Valia
     * @description gets the Loader status
     */
    public getLoaderStatus(): Observable<boolean> {
        return this.loaderStatus;
    }
}
