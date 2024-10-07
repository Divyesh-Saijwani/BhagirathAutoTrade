
/**
 * @description provides the sub error types
 */
export class SubError {
    /** the field for which has been error occur */
    public field?: string;
    /** the sub error message */
    public message?: string;
}
/**
 * provides the Toaster title throughout the application
 */
export enum MessageTitles {
    /** provides Susscess title for success toaster */
    Success = 'Success!',
    /** provides error title for error toaster */
    Error = 'Error!',
    /** provides warning title for warning toaster */
    Warning = 'Warning!',
    /** provides InvalidAccess title for InvalidAccess toaster */
    InvalidAccess = 'Invalid Access'
}

/**
 * `
 * @author: hiren Tandel
 * @description: It indicate status code of the API
 */
export enum Messages {
    /** provide all server comman error message */
    MessageForCommanError = 'Facing issues. Please try again later.',
    /** provide unauthorized error message */
    MessageForUnauthorized = 'You don\'t have permission to access the data for this page.',
    /** provide message if token is expired */
    MessageForUnauthorizedToken = 'Session Expired. Please login again.'

}

