import {
    Show,
    TextField,
    DateField,
    useRecordContext,
} from 'react-admin';
import {
    Card,
    CardContent,
    Typography,
    Grid,
    Chip,
    Box,
    Divider
} from '@mui/material';
import CardMembershipIcon from '@mui/icons-material/CardMembership';
import PersonIcon from '@mui/icons-material/Person';
import BusinessIcon from '@mui/icons-material/Business';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import LinkIcon from '@mui/icons-material/Link';
import FingerprintIcon from '@mui/icons-material/Fingerprint';

const CertificateDetails = () => {
    const record = useRecordContext();
    if (!record) return null;

    const getBlockchainColor = (blockchain: string) => {
        switch (blockchain?.toLowerCase()) {
            case 'ethereum': return '#627eea';
            case 'polygon': return '#8247e5';
            default: return '#757575';
        }
    };

    return (
        <Box sx={{ maxWidth: 900, margin: '0 auto', p: 2 }}>
            {/* Header Card */}
            <Card sx={{ mb: 3, background: 'linear-gradient(135deg, #1a1a2e 0%, #16213e 100%)' }}>
                <CardContent sx={{ textAlign: 'center', py: 4 }}>
                    <CardMembershipIcon sx={{ fontSize: 60, color: '#00d4ff', mb: 2 }} />
                    <Typography variant="h4" sx={{ color: 'white', fontWeight: 700, mb: 1 }}>
                        {record.title}
                    </Typography>
                    <Chip
                        label={record.blockchain}
                        sx={{
                            bgcolor: getBlockchainColor(record.blockchain),
                            color: 'white',
                            fontWeight: 600,
                            fontSize: '0.9rem',
                            px: 2
                        }}
                    />
                </CardContent>
            </Card>

            {/* Details Grid */}
            <Grid container spacing={3}>
                {/* Owner Info */}
                <Grid size={{ xs: 12, md: 6 }}>
                    <Card sx={{ height: '100%' }}>
                        <CardContent>
                            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                                <PersonIcon sx={{ color: '#3b82f6', mr: 1 }} />
                                <Typography variant="h6" fontWeight={600}>Owner</Typography>
                            </Box>
                            <Typography variant="h5" sx={{ color: '#333' }}>
                                {record.ownerName}
                            </Typography>
                        </CardContent>
                    </Card>
                </Grid>

                {/* Issuer Info */}
                <Grid size={{ xs: 12, md: 6 }}>
                    <Card sx={{ height: '100%' }}>
                        <CardContent>
                            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                                <BusinessIcon sx={{ color: '#10b981', mr: 1 }} />
                                <Typography variant="h6" fontWeight={600}>Issuer</Typography>
                            </Box>
                            <Typography variant="h5" sx={{ color: '#333' }}>
                                {record.issuedBy}
                            </Typography>
                        </CardContent>
                    </Card>
                </Grid>

                {/* Dates */}
                <Grid size={{ xs: 12, md: 6 }}>
                    <Card sx={{ height: '100%' }}>
                        <CardContent>
                            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                                <CalendarTodayIcon sx={{ color: '#f59e0b', mr: 1 }} />
                                <Typography variant="h6" fontWeight={600}>Issue Date</Typography>
                            </Box>
                            <Typography variant="body1" sx={{ fontSize: '1.1rem' }}>
                                <DateField source="issuedDate" showTime />
                            </Typography>
                        </CardContent>
                    </Card>
                </Grid>

                <Grid size={{ xs: 12, md: 6 }}>
                    <Card sx={{ height: '100%' }}>
                        <CardContent>
                            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                                <CalendarTodayIcon sx={{ color: '#8b5cf6', mr: 1 }} />
                                <Typography variant="h6" fontWeight={600}>Created At</Typography>
                            </Box>
                            <Typography variant="body1" sx={{ fontSize: '1.1rem' }}>
                                <DateField source="createdAt" showTime />
                            </Typography>
                        </CardContent>
                    </Card>
                </Grid>

                {/* Certificate Hash */}
                <Grid size={12}>
                    <Card>
                        <CardContent>
                            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                                <FingerprintIcon sx={{ color: '#ef4444', mr: 1 }} />
                                <Typography variant="h6" fontWeight={600}>Certificate Hash</Typography>
                            </Box>
                            <Box
                                sx={{
                                    bgcolor: '#f5f5f5',
                                    p: 2,
                                    borderRadius: 2,
                                    fontFamily: 'monospace',
                                    fontSize: '0.95rem',
                                    wordBreak: 'break-all',
                                    color: '#333'
                                }}
                            >
                                {record.certificateHash}
                            </Box>
                        </CardContent>
                    </Card>
                </Grid>

                {/* Blockchain Info */}
                <Grid size={12}>
                    <Card>
                        <CardContent>
                            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                                <LinkIcon sx={{ color: '#06b6d4', mr: 1 }} />
                                <Typography variant="h6" fontWeight={600}>Blockchain Details</Typography>
                            </Box>
                            <Grid container spacing={2}>
                                <Grid size={{ xs: 12, sm: 4 }}>
                                    <Typography variant="body2" color="textSecondary">Network</Typography>
                                    <Chip
                                        label={record.blockchain}
                                        size="small"
                                        sx={{
                                            bgcolor: getBlockchainColor(record.blockchain),
                                            color: 'white',
                                            mt: 0.5
                                        }}
                                    />
                                </Grid>
                                <Grid size={{ xs: 12, sm: 8 }}>
                                    <Typography variant="body2" color="textSecondary">Certificate ID</Typography>
                                    <Typography
                                        variant="body1"
                                        sx={{ fontFamily: 'monospace', mt: 0.5 }}
                                    >
                                        {record.id}
                                    </Typography>
                                </Grid>
                            </Grid>
                        </CardContent>
                    </Card>
                </Grid>
            </Grid>
        </Box>
    );
};

export const CertificateShow = () => (
    <Show>
        <CertificateDetails />
    </Show>
);