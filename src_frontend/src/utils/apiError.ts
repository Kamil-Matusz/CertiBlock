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
    // Users
    'email_already_in_use': 'This email address is already in use.',
    'user_not_found': 'User was not found.',
    'user_role_not_exist': 'This user role does not exist.',
    'account_is_not_active': 'The account is inactive. Unable to sign in.',
    'invalid_role': 'The selected user role is invalid.',

    // Certificates
    'certificate_not_found': 'Certificate was not found.',
    'cetrtificate_for_user_not_found': 'Certificate for this user was not found.',
    'unsupported_blockchain': 'This blockchain is not supported for certificate registration.',
    'blockchain_transaction_failed': 'Certificate registration on the blockchain has failed.',
    'blockchain_configuration': 'Blockchain configuration is invalid. Please contact support.',

    // Ethereum
    'ethereum_balance': 'Failed to get ETH balance for this address.',
    'ethereum_transactions_not_found': 'Ethereum transaction was not found.',
    'ethereum_transactions_by_certificate_id_not_found': 'Ethereum transaction for this certificate was not found.',
    'ethereum_transactions_hash_not_found': 'Ethereum transaction with this hash was not found.',

    // Polygon
    'polygon_balance': 'Failed to get Polygon balance for this address.',
    'polygon_transactions_not_found': 'Polygon transaction was not found.',
    'polygon_transactions_by_certificate_id_not_found': 'Polygon transaction for this certificate was not found.',
    'polygon_transactions_by_hash_not_found': 'Polygon transaction with this hash was not found.',

    // Generic fallback
    'error': 'An error occurred. Please try again.',
};

export function getErrorMessage(code: string, originalMessage: string): string {
    return errorCodeMessages[code] || originalMessage;
}