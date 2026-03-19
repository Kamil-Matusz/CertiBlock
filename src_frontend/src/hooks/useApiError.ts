import { useState, useCallback } from 'react';
import { parseApiError, type ApiErrorItem } from '../utils/apiError';

export interface ApiErrorState {
    message: string;
    code: string;
    allErrors: ApiErrorItem[];
}

export function useApiError() {
    const [error, setError] = useState<ApiErrorState | null>(null);

    const setErrorFromResponse = useCallback(async (response: Response) => {
        const parsedError = await parseApiError(response);
        setError(parsedError);
    }, []);

    const setErrorMessage = useCallback((message: string, code: string = 'unknown_error') => {
        setError({
            message,
            code,
            allErrors: [{ code, message }]
        });
    }, []);

    const clearError = useCallback(() => {
        setError(null);
    }, []);

    const errorMessage = error?.message || '';

    const hasError = error !== null;

    return {
        error,
        errorMessage,
        hasError,
        setErrorFromResponse,
        setErrorMessage,
        clearError
    };
}