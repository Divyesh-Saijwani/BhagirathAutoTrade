/**
 * @description provides the configuration for ALL the Inputs.
 */
export class OverlayInputConfig {
    /**
     * the title for panel
     */
    public title!: string;

    /**
     * the visibility for submit button.
     */
    public isSubmitEnable!: boolean;

    /**
     * determines that form is touched or image is selected or zip file selected for once..?
     */
    public isFormTouched!: boolean;
}
