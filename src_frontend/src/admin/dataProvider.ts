import type {DataProvider} from 'react-admin';
import { API_URL } from '../config.ts';

const apiUrl = 'http://localhost:5126';

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
            const url = `${API_URL}/certificate-service/Certificates/getCertificates?pageIndex=${page}&pageSize=${perPage}`;
            const countUrl = `${API_URL}/certificate-service/Certificates/countCertificates`;

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
            const url = `${API_URL}/ethereum-service/Ethereum/getEthereumTransactions?pageIndex=${page}&pageSize=${perPage}`;
            const countUrl = `${API_URL}/ethereum-service/Ethereum/countEthereumTransactions`;

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
            const url = `${API_URL}/polygon-service/Polygon/getPolygonTransactions?pageIndex=${page}&pageSize=${perPage}`;
            const countUrl = `${API_URL}/polygon-service/Polygon/countPolygonTransactions`;

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
            url = `${apiUrl}/certificate-service/Certificates/${params.id}`;
        } else {
            url = `${apiUrl}/${resource}/${params.id}`;
        }

        const response = await fetch(url, { headers: getHeaders() });
        const data = await response.json();

        return {
            data,
        };
    },

    getMany: async (resource, params) => {
        const url = `${apiUrl}/${resource}?ids=${params.ids.join(',')}`;
        const response = await fetch(url, { headers: getHeaders() });
        const data = await response.json();

        return {
            data,
        };
    },

    getManyReference: async (resource, params) => {
        const { page, perPage } = params.pagination;
        const url = `${apiUrl}/${resource}?${params.target}=${params.id}&page=${page}&perPage=${perPage}`;

        const response = await fetch(url, { headers: getHeaders() });
        const data = await response.json();

        return {
            data: data,
            total: 100,
        };
    },

    create: async (resource, params) => {
        const url = `${apiUrl}/${resource}`;
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
        const url = `${apiUrl}/${resource}/${params.id}`;
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
                fetch(`${apiUrl}/${resource}/${id}`, {
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
            url = `${apiUrl}/certificate-service/Certificates/${params.id}`;
        } else if (resource === 'ethereum-transactions') {
            url = `${apiUrl}/ethereum-service/Ethereum/${params.id}`;
        } else if (resource === 'polygon-transactions') {
            url = `${apiUrl}/polygon-service/Polygon/${params.id}`;
        } else {
            url = `${apiUrl}/${resource}/${params.id}`;
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
                fetch(`${apiUrl}/${resource}/${id}`, {
                    method: 'DELETE',
                    headers: getHeaders(),
                })
            )
        ).then(() => ({
            data: params.ids,
        }));
    },
};