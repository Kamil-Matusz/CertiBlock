import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
    Box,
    Card,
    CardContent,
    TextField,
    Button,
    Typography,
    MenuItem,
    Alert,
    Snackbar
} from '@mui/material';
import { SaveOutlined, ArrowBack } from '@mui/icons-material';

const apiUrl = 'http://localhost:5126';

export const CertificateCreateForm = () => {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        ownerName: '',
        title: '',
        issuedBy: '',
        issuedDate: new Date().toISOString().split('T')[0],
        blockchain: 'Ethereum'
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [showToast, setShowToast] = useState(false);

    const handleChange = (field: string) => (event: React.ChangeEvent<HTMLInputElement>) => {
        setFormData({
            ...formData,
            [field]: event.target.value
        });
        setError('');
    };

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();

        if (!formData.ownerName || !formData.title || !formData.issuedBy) {
            setError('Please fill in all required fields');
            return;
        }

        setLoading(true);
        setError('');

        try {
            const token = localStorage.getItem('token');

            if (!token) {
                setError('You are not logged in. Please login first.');
                setLoading(false);
                return;
            }

            const response = await fetch(`${apiUrl}/certificate-service/Certificates`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({
                    ownerName: formData.ownerName,
                    title: formData.title,
                    issuedBy: formData.issuedBy,
                    issuedDate: new Date(formData.issuedDate).toISOString(),
                    blockchain: formData.blockchain
                }),
            });

            if (!response.ok) {
                if (response.status === 401) {
                    throw new Error('Unauthorized. Please login again.');
                }
                throw new Error('Failed to create certificate');
            }

            setShowToast(true);
            setFormData({
                ownerName: '',
                title: '',
                issuedBy: '',
                issuedDate: new Date().toISOString().split('T')[0],
                blockchain: 'Ethereum'
            });

        } catch (err) {
            setError(err instanceof Error ? err.message : 'Failed to create certificate. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    const handleBack = () => {
        navigate('/admin/certificates');
    };

    const handleToastClose = () => {
        setShowToast(false);
    };

    return (
        <Box
            sx={{
                minHeight: '100vh',
                background: 'linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)',
                padding: 4
            }}
        >
            <Box maxWidth="800px" margin="0 auto">
                {/* Header */}
                <Box mb={3}>
                    <Button
                        startIcon={<ArrowBack />}
                        onClick={handleBack}
                        variant="outlined"
                        sx={{ mb: 2 }}
                    >
                        Back to List
                    </Button>
                    <Typography variant="h4" fontWeight={700}>
                        Create New Certificate
                    </Typography>
                </Box>

                {/* Form Card */}
                <Card sx={{ borderRadius: 3, boxShadow: 4 }}>
                    <CardContent sx={{ p: 4 }}>
                        {error && (
                            <Alert severity="error" sx={{ mb: 3 }}>
                                {error}
                            </Alert>
                        )}

                        <form onSubmit={handleSubmit}>
                            <Box display="flex" flexDirection="column" gap={3}>
                                {/* Owner Name */}
                                <TextField
                                    label="Owner Name"
                                    fullWidth
                                    required
                                    value={formData.ownerName}
                                    onChange={handleChange('ownerName')}
                                    disabled={loading}
                                    placeholder="e.g., John Doe"
                                />

                                {/* Title */}
                                <TextField
                                    label="Certificate Title"
                                    fullWidth
                                    required
                                    value={formData.title}
                                    onChange={handleChange('title')}
                                    disabled={loading}
                                    placeholder="e.g., University Degree"
                                />

                                {/* Issued By */}
                                <TextField
                                    label="Issued By"
                                    fullWidth
                                    required
                                    value={formData.issuedBy}
                                    onChange={handleChange('issuedBy')}
                                    disabled={loading}
                                    placeholder="e.g., Harvard University"
                                />

                                {/* Issued Date */}
                                <TextField
                                    label="Issued Date"
                                    type="date"
                                    fullWidth
                                    required
                                    value={formData.issuedDate}
                                    onChange={handleChange('issuedDate')}
                                    disabled={loading}
                                    InputLabelProps={{ shrink: true }}
                                />

                                {/* Blockchain */}
                                <TextField
                                    label="Blockchain"
                                    select
                                    fullWidth
                                    required
                                    value={formData.blockchain}
                                    onChange={handleChange('blockchain')}
                                    disabled={loading}
                                >
                                    <MenuItem value="Ethereum">Ethereum</MenuItem>
                                    <MenuItem value="Polygon">Polygon</MenuItem>
                                </TextField>

                                {/* Submit Button */}
                                <Box display="flex" gap={2} justifyContent="flex-end" mt={2}>
                                    <Button
                                        variant="outlined"
                                        onClick={handleBack}
                                        disabled={loading}
                                        size="large"
                                    >
                                        Cancel
                                    </Button>
                                    <Button
                                        type="submit"
                                        variant="contained"
                                        disabled={loading}
                                        startIcon={<SaveOutlined />}
                                        size="large"
                                        sx={{
                                            background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                                            '&:hover': {
                                                background: 'linear-gradient(135deg, #5568d3 0%, #63407a 100%)',
                                            },
                                            minWidth: '160px'
                                        }}
                                    >
                                        {loading ? 'Creating...' : 'Create Certificate'}
                                    </Button>
                                </Box>
                            </Box>
                        </form>
                    </CardContent>
                </Card>
            </Box>
            <Snackbar
                open={showToast}
                autoHideDuration={4000}
                onClose={handleToastClose}
                anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
            >
                <Alert
                    onClose={handleToastClose}
                    severity="success"
                    variant="filled"
                    sx={{ width: '100%' }}
                >
                    Certificate created successfully! 🎉
                </Alert>
            </Snackbar>
        </Box>
    );
};