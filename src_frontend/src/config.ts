export const API_URL = 'http://localhost:5126';

// Helper functions for API endpoints
export const apiEndpoints = {
    certificate: (endpoint: string) => `${API_URL}/certificate-service/Certificates/${endpoint}`,
    ethereum: (endpoint: string) => `${API_URL}/ethereum-service/Ethereum/${endpoint}`,
    polygon: (endpoint: string) => `${API_URL}/polygon-service/Polygon/${endpoint}`,
    ethereumMetrics: (endpoint: string) => `${API_URL}/ethereum-service/EthereumMetrics/${endpoint}`,
    polygonMetrics: (endpoint: string) => `${API_URL}/polygon-service/PolygonMetrics/${endpoint}`,
};