import type {DataProvider} from 'react-admin';

const apiUrl = 'http://localhost:5126';

export const dataProvider: DataProvider = {
    getList: async (resource, params) => {
        const { page, perPage } = params.pagination;

        if (resource === 'certificates') {
            const url = `${apiUrl}/certificate-service/Certificates/getCertificates?pageIndex=${page}&pageSize=${perPage}`;
            const countUrl = `${apiUrl}/certificate-service/Certificates/countCertificates`;

            const [dataResponse, countResponse] = await Promise.all([
                fetch(url),
                fetch(countUrl)
            ]);

            const data = await dataResponse.json();
            const total = await countResponse.json();

            return {
                data: data,
                total: total,
            };
        }

        if (resource === 'ethereum-transactions') {
            const url = `${apiUrl}/ethereum-service/Ethereum/getEthereumTransactions?pageIndex=${page}&pageSize=${perPage}`;
            const countUrl = `${apiUrl}/ethereum-service/Ethereum/countEthereumTransactions`;

            const [dataResponse, countResponse] = await Promise.all([
                fetch(url),
                fetch(countUrl)
            ]);

            const data = await dataResponse.json();
            const total = await countResponse.json();

            return {
                data: data,
                total: total,
            };
        }

        if (resource === 'polygon-transactions') {
            const url = `${apiUrl}/polygon-service/Polygon/getPolygonTransactions?pageIndex=${page}&pageSize=${perPage}`;
            const countUrl = `${apiUrl}/polygon-service/Polygon/countPolygonTransactions`;

            const [dataResponse, countResponse] = await Promise.all([
                fetch(url),
                fetch(countUrl)
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
        const url = `${apiUrl}/${resource}/${params.id}`;
        const response = await fetch(url);
        const data = await response.json();

        return {
            data,
        };
    },

    getMany: async (resource, params) => {
        const url = `${apiUrl}/${resource}?ids=${params.ids.join(',')}`;
        const response = await fetch(url);
        const data = await response.json();

        return {
            data,
        };
    },

    getManyReference: async (resource, params) => {
        const { page, perPage } = params.pagination;
        const url = `${apiUrl}/${resource}?${params.target}=${params.id}&page=${page}&perPage=${perPage}`;

        const response = await fetch(url);
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
            headers: {
                'Content-Type': 'application/json',
            },
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
            headers: {
                'Content-Type': 'application/json',
            },
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
                    headers: {
                        'Content-Type': 'application/json',
                    },
                })
            )
        ).then(() => ({
            data: params.ids,
        }));
    },

    delete: async (resource, params) => {
        const url = `${apiUrl}/${resource}/${params.id}`;
        const response = await fetch(url, {
            method: 'DELETE',
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
                })
            )
        ).then(() => ({
            data: params.ids,
        }));
    },
};