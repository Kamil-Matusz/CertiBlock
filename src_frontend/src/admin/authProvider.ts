import type { AuthProvider } from 'react-admin';
import { API_URL } from '../config.ts';

const apiUrl = API_URL;

const decodeJWT = (token: string) => {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
            atob(base64)
                .split('')
                .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                .join('')
        );
        return JSON.parse(jsonPayload);
    } catch {
        return null;
    }
};

export const authProvider: AuthProvider = {
    login: async ({ username, password }) => {
        try {
            const response = await fetch(`${apiUrl}/users-service/Users/signIn`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    email: username,
                    password
                }),
            });

            if (!response.ok) {
                throw new Error('Invalid credentials');
            }

            const data = await response.json();
            const decoded = decodeJWT(data.accessToken);

            localStorage.setItem('token', data.accessToken);
            localStorage.setItem('userId', decoded?.sub || '');
            localStorage.setItem('userRole', decoded?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 'User');

            return Promise.resolve();
        } catch {
            return Promise.reject(new Error('Invalid email or password'));
        }
    },

    logout: () => {
        localStorage.removeItem('token');
        localStorage.removeItem('userId');
        localStorage.removeItem('userRole');
        return Promise.resolve();
    },

    checkError: ({ status }) => {
        if (status === 401 || status === 403) {
            localStorage.removeItem('token');
            localStorage.removeItem('userId');
            localStorage.removeItem('userRole');
            return Promise.reject();
        }
        return Promise.resolve();
    },

    checkAuth: () => {
        const token = localStorage.getItem('token');
        if (!token) {
            return Promise.reject();
        }

        const decoded = decodeJWT(token);
        if (decoded && decoded.exp) {
            const now = Date.now() / 1000;
            if (decoded.exp < now) {
                localStorage.removeItem('token');
                localStorage.removeItem('userId');
                localStorage.removeItem('userRole');
                return Promise.reject();
            }
        }

        return Promise.resolve();
    },

    getIdentity: () => {
        try {
            const token = localStorage.getItem('token');
            const userId = localStorage.getItem('userId');
            const userRole = localStorage.getItem('userRole');

            if (token && userId) {
                return Promise.resolve({
                    id: userId,
                    fullName: `User (${userRole || 'Admin'})`,
                });
            }
            return Promise.reject();
        } catch {
            return Promise.reject();
        }
    },

    getPermissions: () => {
        const userRole = localStorage.getItem('userRole');
        return Promise.resolve(userRole || 'User');
    },
};