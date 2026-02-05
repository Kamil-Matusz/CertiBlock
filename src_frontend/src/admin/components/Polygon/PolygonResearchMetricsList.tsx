import { useState, useEffect } from 'react';
import { apiEndpoints } from '../../../config';
import {
    Box,
    Typography,
    CircularProgress,
    Paper,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Alert
} from '@mui/material';

interface ResearchMetric {
    certificateId: string;
    transactionHash: string;
    transactionCostNative: number;
    transactionCostUsd: number;
    finalizationTimeSeconds: number | null;
    confirmations: number;
    gasUsed: number;
    inclusionTimeSeconds: number;
}

export const PolygonResearchMetricsList = () => {
    const [metrics, setMetrics] = useState<ResearchMetric[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchMetrics = async () => {
            try {
                const token = localStorage.getItem('token');
                const response = await fetch(
                    apiEndpoints.polygonMetrics('researchMetricsForPolygon'),
                    {
                        headers: {
                            'Content-Type': 'application/json',
                            ...(token && { 'Authorization': `Bearer ${token}` })
                        }
                    }
                );

                if (!response.ok) {
                    throw new Error('Failed to fetch Polygon research metrics');
                }

                const data = await response.json();
                setMetrics(data);
            } catch (err) {
                setError(err instanceof Error ? err.message : 'An error occurred');
            } finally {
                setLoading(false);
            }
        };

        fetchMetrics();
    }, []);

    const formatHash = (hash: string) => {
        if (!hash || hash.length < 10) return hash;
        return `${hash.slice(0, 6)}...${hash.slice(-4)}`;
    };

    if (loading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
                <CircularProgress size={48} />
            </Box>
        );
    }

    if (error) {
        return (
            <Box p={3}>
                <Alert severity="error">{error}</Alert>
            </Box>
        );
    }

    if (metrics.length === 0) {
        return (
            <Box p={3}>
                <Alert severity="info">No research metrics available for Polygon</Alert>
            </Box>
        );
    }

    return (
        <Box p={3}>
            <Typography
                variant="h5"
                gutterBottom
                sx={{
                    fontWeight: 600,
                    background: 'linear-gradient(135deg, #8247e5 0%, #c084fc 100%)',
                    WebkitBackgroundClip: 'text',
                    WebkitTextFillColor: 'transparent',
                    mb: 3
                }}
            >
                Polygon Research Metrics
            </Typography>

            <TableContainer
                component={Paper}
                sx={{
                    boxShadow: '0 4px 12px rgba(0,0,0,0.08)',
                    borderRadius: '12px'
                }}
            >
                <Table>
                    <TableHead>
                        <TableRow sx={{ backgroundColor: '#f9fafb' }}>
                            <TableCell sx={{ fontWeight: 600, color: '#374151' }}>Certificate ID</TableCell>
                            <TableCell sx={{ fontWeight: 600, color: '#374151' }}>Transaction Hash</TableCell>
                            <TableCell align="right" sx={{ fontWeight: 600, color: '#374151' }}>Cost (USD)</TableCell>
                            <TableCell align="right" sx={{ fontWeight: 600, color: '#374151' }}>Cost (MATIC)</TableCell>
                            <TableCell align="right" sx={{ fontWeight: 600, color: '#374151' }}>Gas Used</TableCell>
                            <TableCell align="right" sx={{ fontWeight: 600, color: '#374151' }}>Confirmations</TableCell>
                            <TableCell align="right" sx={{ fontWeight: 600, color: '#374151' }}>Inclusion Time (s)</TableCell>
                            <TableCell align="right" sx={{ fontWeight: 600, color: '#374151' }}>Finalization Time (s)</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {metrics.map((metric, index) => (
                            <TableRow
                                key={`${metric.certificateId}-${index}`}
                                sx={{
                                    '&:hover': {
                                        backgroundColor: '#f9fafb'
                                    },
                                    transition: 'background-color 0.2s'
                                }}
                            >
                                <TableCell sx={{ fontFamily: 'monospace', fontSize: '0.875rem' }}>
                                    {formatHash(metric.certificateId)}
                                </TableCell>
                                <TableCell sx={{ fontFamily: 'monospace', fontSize: '0.875rem' }}>
                                    {formatHash(metric.transactionHash)}
                                </TableCell>
                                <TableCell align="right" sx={{ color: '#059669', fontWeight: 500 }}>
                                    ${metric.transactionCostUsd.toFixed(6)}
                                </TableCell>
                                <TableCell align="right" sx={{ fontFamily: 'monospace' }}>
                                    {metric.transactionCostNative.toFixed(10)}
                                </TableCell>
                                <TableCell align="right" sx={{ fontWeight: 500 }}>
                                    {metric.gasUsed.toLocaleString()}
                                </TableCell>
                                <TableCell align="right" sx={{ fontWeight: 500 }}>
                                    {metric.confirmations.toLocaleString()}
                                </TableCell>
                                <TableCell align="right" sx={{ color: '#8247e5', fontWeight: 500 }}>
                                    {metric.inclusionTimeSeconds.toFixed(2)}
                                </TableCell>
                                <TableCell align="right" sx={{ color: '#c084fc', fontWeight: 500 }}>
                                    {metric.finalizationTimeSeconds
                                        ? metric.finalizationTimeSeconds.toFixed(2)
                                        : 'N/A'
                                    }
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>

            <Box mt={2}>
                <Typography variant="body2" color="text.secondary">
                    Total records: {metrics.length}
                </Typography>
            </Box>
        </Box>
    );
};