import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
    Box,
    Card,
    CardContent,
    TextField,
    Button,
    Typography,
    Alert,
    Link,
    FormControlLabel,
    Checkbox,
    MenuItem
} from '@mui/material';
import { PersonAdd as PersonAddIcon } from '@mui/icons-material';

const apiUrl = 'http://localhost:5126';

export const RegisterPage = () => {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        email: '',
        password: '',
        confirmPassword: '',
        role: 'User',
        isActive: true
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState(false);

    const handleChange = (field: string) => (event: React.ChangeEvent<HTMLInputElement>) => {
        setFormData({
            ...formData,
            [field]: event.target.value
        });
        setError('');
    };

    const handleCheckboxChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setFormData({
            ...formData,
            isActive: event.target.checked
        });
    };

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();

        if (!formData.email || !formData.password) {
            setError('Email and password are required');
            return;
        }

        if (formData.password !== formData.confirmPassword) {
            setError('Passwords do not match');
            return;
        }

        if (formData.password.length < 6) {
            setError('Password must be at least 6 characters long');
            return;
        }

        if (formData.password.length > 200) {
            setError('Password cannot be longer than 200 characters');
            return;
        }

        setLoading(true);
        setError('');

        try {
            const response = await fetch(`${apiUrl}/users-service/Users/signUp`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    userId: '00000000-0000-0000-0000-000000000000',
                    email: formData.email,
                    password: formData.password,
                    role: formData.role,
                    isActive: formData.isActive
                }),
            });

            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.message || 'Registration failed');
            }

            setSuccess(true);
            setTimeout(() => {
                navigate('/admin');
            }, 2000);

        } catch (err) {
            setError(err instanceof Error ? err.message : 'Registration failed. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    const handleLoginClick = () => {
        navigate('/admin');
    };

    return (
        <Box
            sx={{
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center',
                minHeight: '100vh',
                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                padding: 2
            }}
        >
            <Card sx={{ maxWidth: 500, width: '100%', borderRadius: 3, boxShadow: 6 }}>
                <CardContent sx={{ p: 4 }}>
                    <Box display="flex" flexDirection="column" alignItems="center" mb={3}>
                        <Box
                            sx={{
                                width: 60,
                                height: 60,
                                borderRadius: '50%',
                                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                mb: 2
                            }}
                        >
                            <PersonAddIcon sx={{ color: 'white', fontSize: 32 }} />
                        </Box>
                        <Typography variant="h4" fontWeight={700} gutterBottom>
                            Create Account
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                            Fill in the details to register
                        </Typography>
                    </Box>

                    {error && (
                        <Alert severity="error" sx={{ mb: 3 }}>
                            {error}
                        </Alert>
                    )}

                    {success && (
                        <Alert severity="success" sx={{ mb: 3 }}>
                            Registration successful! Redirecting to login...
                        </Alert>
                    )}

                    <form onSubmit={handleSubmit}>
                        <TextField
                            label="Email"
                            type="email"
                            fullWidth
                            required
                            value={formData.email}
                            onChange={handleChange('email')}
                            margin="normal"
                            disabled={loading || success}
                            autoComplete="email"
                        />

                        <TextField
                            label="Password"
                            type="password"
                            fullWidth
                            required
                            value={formData.password}
                            onChange={handleChange('password')}
                            margin="normal"
                            disabled={loading || success}
                            autoComplete="new-password"
                            helperText="Minimum 6 characters"
                        />

                        <TextField
                            label="Confirm Password"
                            type="password"
                            fullWidth
                            required
                            value={formData.confirmPassword}
                            onChange={handleChange('confirmPassword')}
                            margin="normal"
                            disabled={loading || success}
                            autoComplete="new-password"
                        />

                        <TextField
                            label="Role"
                            select
                            fullWidth
                            value={formData.role}
                            onChange={handleChange('role')}
                            margin="normal"
                            disabled={loading || success}
                        >
                            <MenuItem value="User">User</MenuItem>
                            <MenuItem value="Admin">Admin</MenuItem>
                        </TextField>

                        <FormControlLabel
                            control={
                                <Checkbox
                                    checked={formData.isActive}
                                    onChange={handleCheckboxChange}
                                    disabled={loading || success}
                                />
                            }
                            label="Active Account"
                            sx={{ mt: 2, mb: 2 }}
                        />

                        <Button
                            type="submit"
                            variant="contained"
                            fullWidth
                            size="large"
                            disabled={loading || success}
                            sx={{
                                mt: 2,
                                py: 1.5,
                                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                                '&:hover': {
                                    background: 'linear-gradient(135deg, #5568d3 0%, #63407a 100%)',
                                }
                            }}
                        >
                            {loading ? 'Registering...' : 'Register'}
                        </Button>
                    </form>

                    <Box mt={3} textAlign="center">
                        <Typography variant="body2" color="text.secondary">
                            Already have an account?{' '}
                            <Link
                                component="button"
                                variant="body2"
                                onClick={handleLoginClick}
                                sx={{
                                    cursor: 'pointer',
                                    fontWeight: 600,
                                    textDecoration: 'none',
                                    '&:hover': { textDecoration: 'underline' }
                                }}
                            >
                                Sign In
                            </Link>
                        </Typography>
                    </Box>
                </CardContent>
            </Card>
        </Box>
    );
};