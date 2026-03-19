export interface ApiErrorItem {
    code: string;
    message: string;
}

export interface ApiErrorResponse {
    errors: ApiErrorItem[];
}

/**
 * Parse API error response and extract error information
 * @param response - Fetch API Response object
 * @returns Promise with parsed error information
 */
export async function parseApiError(response: Response): Promise<{
    message: string;
    code: string;
    allErrors: ApiErrorItem[];
}> {
    try {
        const errorData: ApiErrorResponse = await response.json();

        if (errorData.errors && errorData.errors.length > 0) {
            const firstError = errorData.errors[0];
            return {
                message: firstError.message,
                code: firstError.code,
                allErrors: errorData.errors
            };
        }
    } catch {
    }

    return {
        message: getFallbackMessage(response.status),
        code: 'unknown_error',
        allErrors: []
    };
}

/**
 * Get a user-friendly fallback message based on HTTP status code
 */
function getFallbackMessage(status: number): string {
    switch (status) {
        case 400:
            return 'Invalid request. Please check your input.';
        case 401:
            return 'Unauthorized. Please login again.';
        case 403:
            return 'You do not have permission to perform this action.';
        case 404:
            return 'The requested resource was not found.';
        case 500:
            return 'Server error. Please try again later.';
        default:
            return 'An unexpected error occurred. Please try again.';
    }
}

export const errorCodeMessages: Record<string, string> = {
    'email_already_in_use': 'Ten adres email jest już zajęty.',
    'user_not_found': 'Użytkownik nie został znaleziony.',
    'certificate_not_found': 'Certyfikat nie został znaleziony.',
    'account_is_not_active': 'Konto nie jest aktywne.',
    'invalid_role': 'Nieprawidłowa rola użytkownika.',
    'error': 'Wystąpił błąd. Spróbuj ponownie.',
};

export function getErrorMessage(code: string, originalMessage: string): string {
    return errorCodeMessages[code] || originalMessage;
}