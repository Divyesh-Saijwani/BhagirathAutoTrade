import { ChangeDetectorRef, Component, EventEmitter, Input, Output } from '@angular/core';
import { OverlayInputConfig } from './overlay.model';

@Component({
    selector: 'app-overlay',
    templateUrl: './overlay.component.html',
    styleUrl: './overlay.component.scss'
})
export class OverlayComponent {

    /**
     * provides the configuration for all the inputs
     */
    @Input() public config!: OverlayInputConfig;

    /**
     * submit the form
     */
    // tslint:disable-next-line:no-output-named-after-standard-event
    @Output() public submit!: EventEmitter<null>;

    /**
     * cancel the from activity.
     */
    // tslint:disable-next-line:no-output-named-after-standard-event
    @Output() public cancel!: EventEmitter<null>;

    /**
     * justification for cancel for popup.
     */
    public isCancelOrClose!: boolean;

    constructor(
        private changeDetector: ChangeDetectorRef
    ) {
        this.initProps();
    }

    public ngDoCheck(): void {
        this.changeDetector.detectChanges();
    }

    /**
     * @description emits the submit event when click on submit button.
     */
    public onSubmit(): void {
        this.submit.emit();
    }

    /**
     * @description emits the cancel event when click on cancel button.
     */
    public onCancel(): void {
        this.cancel.emit();
    }

    /**
     * @description when clicks on yes from the popup emits the cancel.
     */
    public onYesClick(): void {
        this.cancel.emit();
    }

    /**
     * @description initializes the properties
     */
    private initProps(): void {
        this.isCancelOrClose = false;
        this.submit = new EventEmitter();
        this.cancel = new EventEmitter();
    }
}
