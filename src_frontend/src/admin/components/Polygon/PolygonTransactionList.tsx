import { useState, useEffect } from 'react';
import { apiEndpoints } from '../../../config.ts';
import {
    List,
    Datagrid,
    TextField,
    DateField,
    FunctionField,
    useNotify,
    useRefresh
} from 'react-admin';
import {
    Chip,
    Button,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    IconButton,
    CircularProgress,
    Box,
    Typography
} from '@mui/material';
import {
    Close as CloseIcon,
    TrendingUp as TrendingUpIcon,
    Delete as DeleteIcon
} from '@mui/icons-material';



interface PolygonMetrics {
    id: string;
    certificateId: string;
    blockchain: string;
    operation: string;
    transactionHash: string;
    dataSizeBytes: number;
    confirmations: number;
    transactionCostUsd: number;
    transactionCostNative: number;
    gasUsed: number;
    gasUtilizationRatio: number;
    inclusionTimeSeconds: number;
    blockNumber: number;
    finalizationTimeSeconds?: number;
    isFinalized: boolean;
}

interface PolygonTransaction {
    id: string;
    certificateId?: string;
    transactionHash?: string;
    status?: string;
    createdAt?: string;
}

interface MetricsModalProps {
    open: boolean;
    onClose: () => void;
    certificateId: string | null;
}

interface MetricCardProps {
    label: string;
    value: string | number;
    suffix?: string;
    gradient: string;
}

interface CustomDeleteButtonProps {
    record: PolygonTransaction;
}

const getStatusColor = (status?: string): 'success' | 'warning' | 'error' | 'default' => {
    switch (status) {
        case 'Confirmed':
            return 'success';
        case 'Pending':
        case 'Submitted':
            return 'warning';
        case 'Failed':
            return 'error';
        default:
            return 'default';
    }
};

const MetricsModal = ({ open, onClose, certificateId }: MetricsModalProps) => {
    const [metrics, setMetrics] = useState<PolygonMetrics | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchMetrics = async () => {
            if (!open || !certificateId) return;

            setLoading(true);
            setError(null);
            setMetrics(null);

            try {
                const response = await fetch(
                    apiEndpoints.polygon(`../PolygonMetrics/getPolygonTransactionMetricsByCertificateId/${certificateId}`)
                );

                if (!response.ok) throw new Error('Failed to fetch metrics');
                const data = await response.json();
                setMetrics(data);
            } catch (err) {
                setError(err instanceof Error ? err.message : 'An error occurred');
            } finally {
                setLoading(false);
            }
        };

        fetchMetrics();
    }, [open, certificateId]);

    const MetricCard = ({ label, value, suffix = '', gradient }: MetricCardProps) => (
        <Box
            sx={{
                background: gradient,
                borderRadius: '12px',
                padding: '20px',
                color: '#1a1a1a',
                boxShadow: '0 4px 12px rgba(0,0,0,0.08)',
                transition: 'transform 0.2s, box-shadow 0.2s',
                '&:hover': {
                    transform: 'translateY(-4px)',
                    boxShadow: '0 6px 18px rgba(0,0,0,0.12)'
                }
            }}
        >
            <Typography variant="caption" sx={{ opacity: 0.8, display: 'block', mb: 1, fontWeight: 500 }}>
                {label}
            </Typography>
            <Typography variant="h5" sx={{ fontWeight: 700, display: 'flex', alignItems: 'baseline', gap: 0.5 }}>
                {value}
                {suffix && (
                    <Typography component="span" variant="body2" sx={{ opacity: 0.8 }}>
                        {suffix}
                    </Typography>
                )}
            </Typography>
        </Box>
    );

    return (
        <Dialog
            open={open}
            onClose={onClose}
            maxWidth="md"
            fullWidth
            PaperProps={{
                sx: {
                    borderRadius: '16px',
                    maxHeight: '90vh'
                }
            }}
        >
            <DialogTitle
                sx={{
                    background: 'linear-gradient(135deg, #8b5cf6 0%, #ec4899 100%)',
                    color: 'white',
                    py: 2.5
                }}
            >
                <Box display="flex" justifyContent="space-between" alignItems="center">
                    <Typography variant="h6" component="span" fontWeight={600}>
                        Polygon Transaction Metrics
                    </Typography>
                    <IconButton
                        onClick={onClose}
                        size="small"
                        sx={{
                            color: 'white',
                            '&:hover': { backgroundColor: 'rgba(255,255,255,0.1)' }
                        }}
                    >
                        <CloseIcon />
                    </IconButton>
                </Box>
            </DialogTitle>

            <DialogContent sx={{ p: 3, backgroundColor: '#f9fafb' }}>
                {loading && (
                    <Box display="flex" justifyContent="center" alignItems="center" py={8}>
                        <CircularProgress size={48} />
                    </Box>
                )}

                {error && (
                    <Box
                        sx={{
                            p: 3,
                            backgroundColor: '#fee2e2',
                            border: '1px solid #fecaca',
                            borderRadius: '12px',
                            color: '#991b1b'
                        }}
                    >
                        <Typography variant="body1">{error}</Typography>
                    </Box>
                )}

                {metrics && !loading && (
                    <Box display="flex" flexDirection="column" gap={3}>
                        {/* Transaction Info */}
                        <Box
                            sx={{
                                backgroundColor: 'white',
                                borderRadius: '12px',
                                padding: 3,
                                boxShadow: '0 2px 8px rgba(0,0,0,0.08)'
                            }}
                        >
                            <Typography variant="h6" fontWeight={600} mb={2} color="primary">
                                Transaction Information
                            </Typography>
                            <Box display="grid" gridTemplateColumns="repeat(2, 1fr)" gap={2}>
                                <Box>
                                    <Typography variant="body2" sx={{ color: '#374151', fontWeight: 600, mb: 1 }}>
                                        Blockchain
                                    </Typography>
                                    <Chip label={metrics.blockchain} color="primary" size="small" sx={{ fontWeight: 600 }} />
                                </Box>
                                <Box>
                                    <Typography variant="body2" sx={{ color: '#374151', fontWeight: 600, mb: 1 }}>
                                        Operation
                                    </Typography>
                                    <Chip label={metrics.operation} color="secondary" size="small" sx={{ fontWeight: 600 }} />
                                </Box>
                                <Box gridColumn="1 / -1">
                                    <Typography variant="body2" sx={{ color: '#374151', fontWeight: 600, mb: 1 }}>
                                        Transaction Hash
                                    </Typography>
                                    <Box
                                        sx={{
                                            fontFamily: 'monospace',
                                            backgroundColor: '#1e293b',
                                            padding: 2,
                                            borderRadius: '8px',
                                            wordBreak: 'break-all',
                                            fontSize: '0.9rem',
                                            color: '#a78bfa',
                                            border: '1px solid #334155',
                                            fontWeight: 500,
                                            letterSpacing: '0.025em'
                                        }}
                                    >
                                        {metrics.transactionHash}
                                    </Box>
                                </Box>
                            </Box>
                        </Box>

                        {/* Metrics Grid */}
                        <Box display="grid" gridTemplateColumns="repeat(auto-fit, minmax(180px, 1fr))" gap={2}>
                            <MetricCard
                                label="Confirmations"
                                value={metrics.confirmations.toLocaleString()}
                                gradient="linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%)"
                            />
                            <MetricCard
                                label="Data Size"
                                value={metrics.dataSizeBytes}
                                suffix="bytes"
                                gradient="linear-gradient(135deg, #fde2e4 0%, #fad2e1 100%)"
                            />
                            <MetricCard
                                label="Gas Used"
                                value={metrics.gasUsed.toLocaleString()}
                                gradient="linear-gradient(135deg, #d1fae5 0%, #a7f3d0 100%)"
                            />
                            <MetricCard
                                label="Gas Utilization"
                                value={metrics.gasUtilizationRatio.toFixed(2)}
                                suffix="%"
                                gradient="linear-gradient(135deg, #fef3c7 0%, #fde68a 100%)"
                            />
                            <MetricCard
                                label="Cost (USD)"
                                value={`$${metrics.transactionCostUsd.toFixed(6)}`}
                                gradient="linear-gradient(135deg, #e0e7ff 0%, #c7d2fe 100%)"
                            />
                            <MetricCard
                                label="Cost (MATIC)"
                                value={metrics.transactionCostNative.toFixed(10)}
                                gradient="linear-gradient(135deg, #ede9fe 0%, #ddd6fe 100%)"
                            />
                            <MetricCard
                                label="Block Number"
                                value={metrics.blockNumber.toLocaleString()}
                                gradient="linear-gradient(135deg, #fee2e2 0%, #fecaca 100%)"
                            />
                            <MetricCard
                                label="Inclusion Time"
                                value={metrics.inclusionTimeSeconds.toFixed(2)}
                                suffix="sec"
                                gradient="linear-gradient(135deg, #cffafe 0%, #a5f3fc 100%)"
                            />
                            {metrics.finalizationTimeSeconds != null && (
                                <MetricCard
                                    label="Finalization Time"
                                    value={metrics.finalizationTimeSeconds.toFixed(2)}
                                    suffix="sec"
                                    gradient="linear-gradient(135deg, #f3e8ff 0%, #e9d5ff 100%)"
                                />
                            )}
                            <MetricCard
                                label="Finalized"
                                value={metrics.isFinalized ? 'Yes' : 'No'}
                                gradient={metrics.isFinalized
                                    ? "linear-gradient(135deg, #d1fae5 0%, #a7f3d0 100%)"
                                    : "linear-gradient(135deg, #fecaca 0%, #fca5a5 100%)"}
                            />
                        </Box>
                    </Box>
                )}
            </DialogContent>
        </Dialog>
    );
};

const CustomDeleteButton = ({ record }: CustomDeleteButtonProps) => {
    const [confirmOpen, setConfirmOpen] = useState(false);
    const [isDeleting, setIsDeleting] = useState(false);
    const notify = useNotify();
    const refresh = useRefresh();

    const handleDeleteClick = () => {
        setConfirmOpen(true);
    };

    const handleConfirmDelete = async () => {
        if (!record.certificateId) {
            notify('Certificate ID not found', { type: 'error' });
            setConfirmOpen(false);
            return;
        }

        setIsDeleting(true);

        try {
            const token = localStorage.getItem('token');
            const response = await fetch(
                apiEndpoints.polygon(`deleteTransactionByCertificateId/${record.certificateId}`),
                {
                    method: 'DELETE',
                    headers: {
                        'Content-Type': 'application/json',
                        ...(token && { 'Authorization': `Bearer ${token}` })
                    }
                }
            );

            if (response.ok) {
                notify('Transaction deleted successfully', { type: 'success' });
                refresh();
            } else {
                const errorData = await response.json().catch(() => ({}));
                notify(errorData.message || 'Error deleting transaction', { type: 'error' });
            }
        } catch {
            notify('Network error while deleting transaction', { type: 'error' });
        } finally {
            setIsDeleting(false);
            setConfirmOpen(false);
        }
    };

    const handleCancelDelete = () => {
        setConfirmOpen(false);
    };

    return (
        <>
            <Button
                variant="outlined"
                color="error"
                size="small"
                startIcon={<DeleteIcon />}
                onClick={handleDeleteClick}
                disabled={!record.certificateId || isDeleting}
            >
                Delete
            </Button>

            <Dialog
                open={confirmOpen}
                onClose={handleCancelDelete}
                maxWidth="xs"
                fullWidth
            >
                <DialogTitle>Confirm Delete</DialogTitle>
                <DialogContent>
                    <Typography>
                        Are you sure you want to delete the transaction for certificate{' '}
                        <strong>{record.certificateId}</strong>?
                    </Typography>
                    <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                        This action cannot be undone.
                    </Typography>
                </DialogContent>
                <DialogActions sx={{ px: 3, pb: 2 }}>
                    <Button onClick={handleCancelDelete} disabled={isDeleting}>
                        Cancel
                    </Button>
                    <Button
                        onClick={handleConfirmDelete}
                        color="error"
                        variant="contained"
                        disabled={isDeleting}
                        startIcon={isDeleting ? <CircularProgress size={16} /> : null}
                    >
                        {isDeleting ? 'Deleting...' : 'Delete'}
                    </Button>
                </DialogActions>
            </Dialog>
        </>
    );
};

export const PolygonTransactionList = () => {
    const [modalOpen, setModalOpen] = useState(false);
    const [selectedCertificateId, setSelectedCertificateId] = useState<string | null>(null);

    const handleOpenMetrics = (certificateId: string) => {
        setSelectedCertificateId(certificateId);
        setModalOpen(true);
    };

    const handleCloseMetrics = () => {
        setModalOpen(false);
        setSelectedCertificateId(null);
    };

    return (
        <>
            <List>
                <Datagrid bulkActionButtons={false}>
                    <TextField source="certificateId" label="Certificate ID" />
                    <TextField
                        source="transactionHash"
                        label="Transaction Hash"
                        sx={{
                            maxWidth: '200px',
                            overflow: 'hidden',
                            textOverflow: 'ellipsis',
                            whiteSpace: 'nowrap',
                            fontFamily: 'monospace'
                        }}
                    />
                    <FunctionField
                        label="Transaction Status"
                        render={(record: PolygonTransaction) => (
                            <Chip
                                label={record?.status || 'Unknown'}
                                color={getStatusColor(record?.status)}
                                size="small"
                            />
                        )}
                    />
                    <DateField source="createdAt" label="Created At" showTime />
                    <FunctionField
                        label="Actions"
                        render={(record: PolygonTransaction) => (
                            <div style={{ display: 'flex', gap: '8px' }}>
                                <Button
                                    variant="contained"
                                    size="small"
                                    startIcon={<TrendingUpIcon />}
                                    onClick={() => record.certificateId && handleOpenMetrics(record.certificateId)}
                                    disabled={!record.certificateId}
                                    sx={{
                                        background: 'linear-gradient(135deg, #8b5cf6 0%, #ec4899 100%)',
                                        '&:hover': {
                                            background: 'linear-gradient(135deg, #7c3aed 0%, #db2777 100%)'
                                        }
                                    }}
                                >
                                    Metrics
                                </Button>
                                <CustomDeleteButton record={record} />
                            </div>
                        )}
                    />
                </Datagrid>
            </List>

            <MetricsModal
                open={modalOpen}
                onClose={handleCloseMetrics}
                certificateId={selectedCertificateId}
            />
        </>
    );
};