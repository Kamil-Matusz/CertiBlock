import type { DataProvider } from 'react-admin';
import { API_URL, apiEndpoints } from '../config.ts';

const getHeaders = () => {
    const token = localStorage.getItem('token');
    return {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` })
    };
};

export const dataProvider: DataProvider = {
    getList: async (resource, params) => {
        const { page, perPage } = params.pagination;

        if (resource === 'certificates') {
            const url = apiEndpoints.certificate(`getCertificates?pageIndex=${page}&pageSize=${perPage}`);
            const countUrl = apiEndpoints.certificate('countCertificates');

            const [dataResponse, countResponse] = await Promise.all([
                fetch(url, { headers: getHeaders() }),
                fetch(countUrl, { headers: getHeaders() })
            ]);

            const data = await dataResponse.json();
            const total = await countResponse.json();

            return {
                data: data,
                total: total,
            };
        }

        if (resource === 'ethereum-transactions') {
            const url = apiEndpoints.ethereum(`getEthereumTransactions?pageIndex=${page}&pageSize=${perPage}`);
            const countUrl = apiEndpoints.ethereum('countEthereumTransactions');

            const [dataResponse, countResponse] = await Promise.all([
                fetch(url, { headers: getHeaders() }),
                fetch(countUrl, { headers: getHeaders() })
            ]);

            const data = await dataResponse.json();
            const total = await countResponse.json();

            return {
                data: data,
                total: total,
            };
        }

        if (resource === 'polygon-transactions') {
            const url = apiEndpoints.polygon(`getPolygonTransactions?pageIndex=${page}&pageSize=${perPage}`);
            const countUrl = apiEndpoints.polygon('countPolygonTransactions');

            const [dataResponse, countResponse] = await Promise.all([
                fetch(url, { headers: getHeaders() }),
                fetch(countUrl, { headers: getHeaders() })
            ]);

            const data = await dataResponse.json();
            const total = await countResponse.json();

            return {
                data: data,
                total: total,
            };
        }

        return {
            data: [],
            total: 0,
        };
    },

    getOne: async (resource, params) => {
        let url: string;

        if (resource === 'certificates') {
            url = apiEndpoints.certificate(String(params.id));
        } else {
            url = `${API_URL}/${resource}/${params.id}`;
        }

        const response = await fetch(url, { headers: getHeaders() });
        const data = await response.json();

        return {
            data,
        };
    },

    getMany: async (resource, params) => {
        const url = `${API_URL}/${resource}?ids=${params.ids.join(',')}`;
        const response = await fetch(url, { headers: getHeaders() });
        const data = await response.json();

        return {
            data,
        };
    },

    getManyReference: async (resource, params) => {
        const { page, perPage } = params.pagination;
        const url = `${API_URL}/${resource}?${params.target}=${params.id}&page=${page}&perPage=${perPage}`;

        const response = await fetch(url, { headers: getHeaders() });
        const data = await response.json();

        return {
            data: data,
            total: 100,
        };
    },

    create: async (resource, params) => {
        const url = `${API_URL}/${resource}`;
        const response = await fetch(url, {
            method: 'POST',
            body: JSON.stringify(params.data),
            headers: getHeaders(),
        });
        const data = await response.json();

        return {
            data,
        };
    },

    update: async (resource, params) => {
        const url = `${API_URL}/${resource}/${params.id}`;
        const response = await fetch(url, {
            method: 'PUT',
            body: JSON.stringify(params.data),
            headers: getHeaders(),
        });
        const data = await response.json();

        return {
            data,
        };
    },

    updateMany: async (resource, params) => {
        return Promise.all(
            params.ids.map(id =>
                fetch(`${API_URL}/${resource}/${id}`, {
                    method: 'PUT',
                    body: JSON.stringify(params.data),
                    headers: getHeaders(),
                })
            )
        ).then(() => ({
            data: params.ids,
        }));
    },

    delete: async (resource, params) => {
        let url: string;

        if (resource === 'certificates') {
            url = apiEndpoints.certificate(String(params.id));
        } else if (resource === 'ethereum-transactions') {
            url = apiEndpoints.ethereum(String(params.id));
        } else if (resource === 'polygon-transactions') {
            url = apiEndpoints.polygon(String(params.id));
        } else {
            url = `${API_URL}/${resource}/${params.id}`;
        }

        const response = await fetch(url, {
            method: 'DELETE',
            headers: getHeaders(),
        });
        const data = await response.json();

        return {
            data,
        };
    },

    deleteMany: async (resource, params) => {
        return Promise.all(
            params.ids.map(id =>
                fetch(`${API_URL}/${resource}/${id}`, {
                    method: 'DELETE',
                    headers: getHeaders(),
                })
            )
        ).then(() => ({
            data: params.ids,
        }));
    },
};