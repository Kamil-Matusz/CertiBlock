import { useState, useEffect } from 'react';
import { apiEndpoints } from '../../../config.ts';
import {
    Chip,
    Box,
    Typography,
    CircularProgress,
    Alert,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
    useTheme
} from '@mui/material';

interface PolygonResearchMetric {
    id: string;
    certificateId: string;
    blockchain: number | string;
    operation: number | string;
    transactionHash: string;
    dataSizeBytes: number;
    confirmations: number;
    gasUsed: number;
    transactionCostUsd: number;
    inclusionTimeSeconds: number;
    blockNumber: number;
    finalizationTimeSeconds?: number;
}

const getBlockchainLabel = (blockchain: number | string): string => {
    if (typeof blockchain === 'string') return blockchain;
    const map: Record<number, string> = {
        0: 'Ethereum',
        1: 'Polygon',
    };
    return map[blockchain] || 'Unknown';
};

const getOperationLabel = (operation: number | string): string => {
    if (typeof operation === 'string') return operation;
    const map: Record<number, string> = {
        0: 'Register',
        1: 'Update',
        2: 'Verify',
    };
    return map[operation] || 'Unknown';
};

const getBlockchainColor = (blockchain: number | string): 'primary' | 'secondary' => {
    const label = getBlockchainLabel(blockchain).toLowerCase();
    if (label.includes('ethereum')) return 'primary';
    if (label.includes('polygon')) return 'secondary';
    return 'primary';
};

const getOperationColor = (operation: number | string): 'success' | 'info' | 'warning' => {
    const label = getOperationLabel(operation).toLowerCase();
    if (label.includes('register')) return 'success';
    if (label.includes('update')) return 'info';
    if (label.includes('verify')) return 'warning';
    return 'info';
};

export const PolygonResearchMetrics = () => {
    const [metrics, setMetrics] = useState<PolygonResearchMetric[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const theme = useTheme();

    useEffect(() => {
        const fetchMetrics = async () => {
            setLoading(true);
            setError(null);

            try {
                const response = await fetch(
                    apiEndpoints.polygon('../PolygonMetrics/getPolygonResearchMetrics')
                );

                if (!response.ok) throw new Error('Failed to fetch research metrics');
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

    return (
        <Box sx={{ p: 3 }}>
            <Box
                sx={{
                    background: 'linear-gradient(135deg, #8b5cf6 0%, #ec4899 100%)',
                    color: 'white',
                    padding: 3,
                    borderRadius: '8px',
                    mb: 3
                }}
            >
                <Typography variant="h5" fontWeight={600}>
                    Polygon Research Metrics
                </Typography>
                <Typography variant="body2" sx={{ opacity: 0.9, mt: 1 }}>
                    Comprehensive metrics for research analysis ({metrics.length} records)
                </Typography>
            </Box>

            {metrics.length === 0 ? (
                <Box
                    sx={{
                        textAlign: 'center',
                        py: 8,
                        backgroundColor: theme.palette.mode === 'dark' ? '#2a2a2a' : '#f9fafb',
                        borderRadius: '8px'
                    }}
                >
                    <Typography variant="h6" color="text.secondary">
                        No research metrics available yet
                    </Typography>
                </Box>
            ) : (
                <TableContainer
                    component={Paper}
                    sx={{
                        boxShadow: '0 2px 8px rgba(0,0,0,0.08)'
                    }}
                >
                    <Table>
                        <TableHead>
                            <TableRow sx={{
                                backgroundColor: theme.palette.mode === 'dark'
                                    ? 'rgba(255, 255, 255, 0.05)'
                                    : '#f9fafb'
                            }}>
                                <TableCell sx={{ fontWeight: 600 }}>Certificate ID</TableCell>
                                <TableCell sx={{ fontWeight: 600 }}>Blockchain</TableCell>
                                <TableCell sx={{ fontWeight: 600 }}>Operation</TableCell>
                                <TableCell sx={{ fontWeight: 600 }}>Transaction Hash</TableCell>
                                <TableCell sx={{ fontWeight: 600 }} align="right">Data Size (bytes)</TableCell>
                                <TableCell sx={{ fontWeight: 600 }} align="right">Confirmations</TableCell>
                                <TableCell sx={{ fontWeight: 600 }} align="right">Gas Used</TableCell>
                                <TableCell sx={{ fontWeight: 600 }} align="right">Cost (USD)</TableCell>
                                <TableCell sx={{ fontWeight: 600 }} align="right">Inclusion Time</TableCell>
                                <TableCell sx={{ fontWeight: 600 }} align="right">Block Number</TableCell>
                                <TableCell sx={{ fontWeight: 600 }} align="right">Finalization Time</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {metrics.map((metric) => (
                                <TableRow
                                    key={metric.id}
                                    sx={{
                                        '&:hover': {
                                            backgroundColor: theme.palette.mode === 'dark'
                                                ? 'rgba(255, 255, 255, 0.05)'
                                                : '#f9fafb'
                                        },
                                        transition: 'background-color 0.2s'
                                    }}
                                >
                                    <TableCell sx={{
                                        fontFamily: 'monospace',
                                        fontSize: '0.85rem',
                                        color: theme.palette.text.secondary
                                    }}>
                                        {metric.certificateId}
                                    </TableCell>
                                    <TableCell>
                                        <Chip
                                            label={getBlockchainLabel(metric.blockchain)}
                                            color={getBlockchainColor(metric.blockchain)}
                                            size="small"
                                            sx={{ fontWeight: 600 }}
                                        />
                                    </TableCell>
                                    <TableCell>
                                        <Chip
                                            label={getOperationLabel(metric.operation)}
                                            color={getOperationColor(metric.operation)}
                                            size="small"
                                            sx={{ fontWeight: 600 }}
                                        />
                                    </TableCell>
                                    <TableCell
                                        sx={{
                                            maxWidth: '150px',
                                            overflow: 'hidden',
                                            textOverflow: 'ellipsis',
                                            whiteSpace: 'nowrap',
                                            fontFamily: 'monospace',
                                            fontSize: '0.85rem',
                                            color: theme.palette.primary.main
                                        }}
                                    >
                                        {metric.transactionHash}
                                    </TableCell>
                                    <TableCell align="right">
                                        {metric.dataSizeBytes.toLocaleString()}
                                    </TableCell>
                                    <TableCell align="right">
                                        {metric.confirmations.toLocaleString()}
                                    </TableCell>
                                    <TableCell align="right">
                                        {metric.gasUsed.toLocaleString()}
                                    </TableCell>
                                    <TableCell align="right">
                                        <Box
                                            sx={{
                                                fontWeight: 600,
                                                color: theme.palette.success.main,
                                                fontFamily: 'monospace'
                                            }}
                                        >
                                            ${metric.transactionCostUsd.toFixed(6)}
                                        </Box>
                                    </TableCell>
                                    <TableCell align="right" sx={{ fontFamily: 'monospace' }}>
                                        {metric.inclusionTimeSeconds.toFixed(2)}s
                                    </TableCell>
                                    <TableCell align="right">
                                        {metric.blockNumber.toLocaleString()}
                                    </TableCell>
                                    <TableCell align="right" sx={{ fontFamily: 'monospace' }}>
                                        {metric.finalizationTimeSeconds != null
                                            ? `${metric.finalizationTimeSeconds.toFixed(2)}s`
                                            : 'N/A'}
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            )}

        </Box>
    );
};
